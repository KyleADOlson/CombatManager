using System;
using CombatManager;
using System.Collections.Generic;
using Android.Widget;

namespace CombatManagerDroid
{
    public class RuleFragment: LookupFragment<Rule>
    {
        Button _SubtypeButton;

        String _Type = "All";
        String _Subtype = "All";
        List<String> _SubtypesList = new List<string>();


        protected override List<Rule> GetItems ()
        {
            return new List<Rule>(Rule.Rules);
        }

        protected override string ItemHtml (Rule item)
        {
            return RuleHtmlCreator.CreateHtml(item);
        }

        protected override string ItemName (Rule item)
        {
            return item.Name;
        }

        protected override bool CustomFilterItem(Rule item)
        {
            return TypeFilter(item) && SubtypeFilter(item);
        }

        bool TypeFilter(Rule item)
        {
            return _Type == "All" || item.Type == _Type; 
        }

        bool SubtypeFilter(Rule item)
        {
            return _Subtype == "All" || item.Subtype == _Subtype;
        }

        protected override void BuildFilters()
        {
            Button b;

            b = BuildFilterButton("Type", 180);

            List<String> types = new List<string>( Rule.Types );
            types.Insert(0, "All");
            PopupUtils.AttachButtonStringPopover("Classes", b, 
                                                 types, 
                                                 0, (r1, index, val)=>
                                                 {
                _Type = val;
                UpdateSubtypeButton();
                UpdateFilter();

            });

            _SubtypeButton = BuildFilterButton("Subtype", 180);
            _SubtypesList.Add("All");
            PopupUtils.AttachButtonStringPopover("Classes", _SubtypeButton, 
                                                 _SubtypesList, 
                                                 0, (r1, index, val) =>
            {
                _Subtype = val;
                UpdateFilter();

            });

            UpdateSubtypeButton();
        }

        void UpdateSubtypeButton()
        {
            if (_Type == "All")
            {
                _SubtypeButton.Visibility = Android.Views.ViewStates.Invisible;
                _Subtype = "All";
            }
            else
            {
                if (!Rule.Subtypes.ContainsKey(_Type))
                {
                    
                    _SubtypeButton.Visibility = Android.Views.ViewStates.Invisible;
                    _Subtype = "All";
                }
                else
                {
                    _Subtype = "All";
                    _SubtypeButton.Text = "All";
                    _SubtypeButton.Visibility = Android.Views.ViewStates.Visible;
                    _SubtypesList.Clear();
                    _SubtypesList.Add("All");
                    _SubtypesList.AddRange(Rule.Subtypes[_Type].Values);
                }
            }
        }
    }
}

