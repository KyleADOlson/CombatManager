/*
 *  ButtonPropertyManager.cs
 *
 *  Copyright (C) 2010-2012 Kyle Olson, kyle@kyleolson.com
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU General Public License
 *  as published by the Free Software Foundation; either version 2
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using CombatManager;
using System.ComponentModel;
using CoreGraphics;
using System.Reflection;



namespace CombatManagerMono
{
	public delegate string PropertyFormatDelegate(object property);
	
	public class ButtonPropertyManager
	{
		INotifyPropertyChanged _PropertyObject;
		UIButton _Button;
		UIView _DialogParent;
		PropertyInfo _Property;
		PropertyFormatDelegate _FormatDelegate;
		bool _Multiline;
        bool _StringList;
		String _Title;
		ButtonStringPopover _ValueListPopover;
		HDEditorDialog _HDDialog;
        UITextView _TextView;
		
		int _MinIntValue = int.MinValue;
		int _MaxIntValue = int.MaxValue;
		
		TextBoxDialog _TextBoxDialog;
		
		List<KeyValuePair<object, string>> _ValueList;
		
		
		public ButtonPropertyManager(UIButton button)
		{
			_Button = button;
			_Button.TouchUpInside += Handle_ButtonTouchUpInside;
		}
		
				
		public ButtonPropertyManager (UIButton button, UIView dialogParent, INotifyPropertyChanged propertyObject, String property)
		{
			_PropertyObject = propertyObject;
			_Property = propertyObject.GetType().GetProperty(property);
			_DialogParent = dialogParent;
			_Button = button;
			_Button.TouchUpInside += Handle_ButtonTouchUpInside;
			_PropertyObject.PropertyChanged += Handle_PropertyObjectPropertyChanged;
			
			UpdateButton();
		}

		void Handle_ButtonTouchUpInside (object sender, EventArgs e)
		{
			if (_ValueList == null)
				{
                if (_Property.PropertyType == typeof(string))
                {
                    _TextBoxDialog = new TextBoxDialog();
                    _TextBoxDialog.HeaderText = DisplayTitle;
                    _TextBoxDialog.Value = (string)Value;
                    _TextBoxDialog.SingleLine = !_Multiline;
                    _TextBoxDialog.OKClicked += Handle_TextBoxDialogOKClicked;
                    _DialogParent.AddSubview(_TextBoxDialog.View);
                }
                else if (_Property.PropertyType == typeof(int) || _Property.PropertyType == typeof(int?))
                {
                    NumberModifyPopover pop = new NumberModifyPopover();
                    pop.ShowOnView(_Button);
                    if (_Property.PropertyType == typeof(int?))
                    {
                        pop.Value = (int?)Value;
                    }
                    else
                    {
						
                        pop.Value = (int)Value;
                    }
                    pop.ValueType = DisplayTitle;
                    pop.Title = DisplayTitle;
                    pop.Data = _PropertyObject;
                    pop.Nullable = (_Property.PropertyType == typeof(int?));
                    pop.NumberModified += HandlePopNumberModified;
                }
                else if (_Property.PropertyType == typeof(DieRoll))
                {
                    _HDDialog = new HDEditorDialog();
                    _HDDialog.HeaderText = DisplayTitle;
                    _HDDialog.DieRoll = (DieRoll)Value;
                    DialogParent.AddSubview(_HDDialog.View);
                    _HDDialog.OKClicked += Handle_HDDialogOKClicked;
                }
                else if (_Property.PropertyType == typeof(bool))
                {
                    Value = !(bool)Value;
                    UpdateButton();
                }
			}
		}

		void Handle_HDDialogOKClicked (object sender, EventArgs e)
		{
			Value = _HDDialog.DieRoll;
		}
		
		
		void Handle_ValueListPopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
            if (StringList)
            {
                List<String> values = ((String)Value).Tokenize(',');

                String clicked = (String)e.Tag;
                if (values.Contains(clicked))
                {
                    if (values.Count > 1 || AllowEmptyStringList)
                    {
                        values.Remove(clicked);
                    }
                }
                else
                {
                    values.Add(clicked);
                }
                Value = values.ToTokenString(',');
            }
            else
            {
                Value = e.Tag;
            }

            UpdateButton();
		}

        void Handle_ValueListWillShowPopover(object sender, WillShowPopoverEventArgs e)
        {
            if (StringList)
            {
                List<String> values = ((String)Value).Tokenize(',');

                foreach (ButtonStringPopoverItem item in _ValueListPopover.Items)
                {
                    item.Selected = values.Contains((string)item.Tag);
                }

            }
        }

		void HandlePopNumberModified (object sender, NumberModifyEventArgs args)
		{
			NumberModifyPopover pop = (NumberModifyPopover)sender;
			
			
			if (args.Set)
			{
				if ( (_Property.PropertyType == typeof(int?)))
				{
					int? val = (int?)args.Value;
					val = BoundInt(val, MinIntValue, MaxIntValue);
					Value = val;
				}
				else
				{
					int val = ((int?)args.Value).Value;
					val = BoundInt(val, MinIntValue, MaxIntValue);
					Value = val;
				}
			}
			else
			{
				if (Value != null)
				{
					if ( (_Property.PropertyType == typeof(int?)))
					{
						int? val = (int?)Value;
						val += args.Value.Value;
						val = BoundInt(val, MinIntValue, MaxIntValue);
						Value = val;
					}
					else
					{
						int val = (int)Value;
						val += args.Value.Value;
						val = BoundInt(val, MinIntValue, MaxIntValue);
						Value = val;
					}
				}
			}
			
			if (_Property.PropertyType == typeof(int?))
			{
				pop.Value = (int?)Value;
			}
			else
			{
				pop.Value = (int)Value;
			}
			UpdateButton();	
		}
		
		static int BoundInt(int val, int min, int max)
		{
			if (val < min)
			{
				val = min;
			}
			if (val > max)
			{
				val = max;
			}
			return val;
		}
		
		static int? BoundInt(int? val, int min, int max)
		{
			if (val != null)
			{
				if (val.Value < min)
				{
					val = min;
				}
				if (val.Value > max)
				{
					val = max;
				}
			}
			return val;
		}
		
		void Handle_TextBoxDialogOKClicked (object sender, EventArgs e)
		{
			if (_Property.PropertyType == typeof(string))
			{
				Value = _TextBoxDialog.Value;
			}
			
			UpdateButton();
		}

		void Handle_PropertyObjectPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			UpdateButton();
		}
		
		void UpdateButton()
		{
            if (!Multiline)
            {
                _Button.SetText(CurrentText);
            }
            if (_TextView != null)
            {
                _TextView.Text = CurrentText;
            }

            if (_Property.PropertyType == typeof(bool))
            {
                if ((bool)Value)
                {
                    _Button.SetImage(UIImage.FromFile("Images/External/CheckBox.png"), UIControlState.Normal);

                }
                else
                {

                    _Button.SetImage(UIImage.FromFile("Images/External/CheckBoxUnchecked.png"), UIControlState.Normal);

                }
            }
		}
		
		string CurrentText
		{
			get
			{
				string text = "";
                if (_FormatDelegate != null)
                {
                    text = _FormatDelegate(Value);	
                }
                if (_Property.PropertyType == typeof(bool))
                {
                    text = DisplayTitle;
                }
                else if (Value != null)
                {
                    text = Value.ToString();
                }
                else if (_Property.PropertyType == typeof(int?))
                {
                    text = "-";	
                }
				return text;
			}
			
		}
		
		object Value
		{
			get
			{
				MethodInfo info = _Property.GetGetMethod();	
				return info.Invoke(_PropertyObject, new object[]{});
			}
			set
			{
				MethodInfo info = _Property.GetSetMethod();
				info.Invoke(_PropertyObject, new object[] {value});
			}
		}
		
		public UIButton Button
		{
			get
			{
				return _Button;
			}
		}
		
		public bool Multiline
		{
			get
			{
				return _Multiline;
			}
			set
			{
                if (_Multiline != value)
                {
				    _Multiline = value;
                    if (_TextView == null)
                    {
                        _TextView = new UITextView();
                        _TextView.Frame = _Button.Bounds;
                        _TextView.UserInteractionEnabled = false;
                        _Button.Add(_TextView);
                        _Button.BringSubviewToFront(_TextView);
                        _TextView.Text = _Button.TitleLabel.Text;
                        _Button.SetText("");
                    }
                    _TextView.Hidden = !_Multiline;
                
                }
			}
		}
		
		public UIView DialogParent
		{
			get
			{
				return _DialogParent;
			}
			set
			{
				_DialogParent = value;
			}
		}
		
		public PropertyFormatDelegate FormatDelegate
		{
			get
			{
				return _FormatDelegate;
			}
			set
			{
				_FormatDelegate = value;
				
				UpdateButton();
			}
		}
		
		public int MinIntValue
		{
			get
			{
				return _MinIntValue;
			}
			set
			{
				_MinIntValue = value;
			}
		}
		
		public int MaxIntValue
		{
			get
			{
				return _MaxIntValue;
			}
			set
			{
				_MaxIntValue = value;
			}
		}

        public bool StringList
        {
            get
            {
                return _StringList;
            }
            set
            {
                _StringList = value;
            }
        }

        public bool AllowEmptyStringList { get; set;}

		public string DisplayTitle
		{
			get
			{
				if (_Title == null)
				{
					return _Property.Name;
				}
				return _Title;
			}
		}
		
		public string Title
		{
			get
			{
				return _Title;
			}
			set
			{
				_Title = value;
			}
		}

        public UITextView TextView
        {
            get
            {
                return _TextView;
            }
        }
		
		public List<KeyValuePair<object, string>> ValueList
		{
			get
			{
				return _ValueList;
			}
			set
			{
				if (value != _ValueList)
				{
					if (_ValueList != null)
					{
						_ValueListPopover.Button = null;
						_ValueListPopover = null;
					}
					_ValueList = value;
					
					if (_ValueList != null)
					{	
						_ValueListPopover = new ButtonStringPopover(_Button);
						foreach (KeyValuePair<object, string> pair in _ValueList)
						{
							_ValueListPopover.Items.Add(new ButtonStringPopoverItem() {Text = pair.Value, Tag = pair.Key});		
						}
                        _ValueListPopover.WillShowPopover += Handle_ValueListWillShowPopover;
						_ValueListPopover.ItemClicked += Handle_ValueListPopoverItemClicked;
					}
						
					
				}
			}

		}
	}
}

