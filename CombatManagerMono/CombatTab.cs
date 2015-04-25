/*
 *  CombatTab.cs
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
using CoreGraphics;

namespace CombatManagerMono
{
	public class CombatTab: CMTab
	{
		CombatListView _CombatList;
		CharacterListView _PlayerList;
		CharacterListView _MonsterList;
		UIWebView _MonsterView;
        DieRollerView _DieView;
		
		Character _SelectedCharacter;
		
		
		public CombatTab (CombatState state) : base (state)
		{
			_CombatList = new CombatListView();
			//
			
			AddSubview(_CombatList);
			_PlayerList = new CharacterListView(CombatState, false);
			_PlayerList.CharacterSelectionChanged += HandlePlayerListCharacterSelectionChanged;
			AddSubview(_PlayerList);
			_MonsterList = new CharacterListView(CombatState, true);
			_MonsterList.CharacterSelectionChanged += HandlePlayerListCharacterSelectionChanged;
			AddSubview(_MonsterList);
			
			_MonsterView = new UIWebView(new CGRect(0, 0, 100, 100));
			_MonsterView.BackgroundColor = UIColor.Brown;
			_MonsterView.LoadHtmlString("<html></html>", new NSUrl("http://localhost/"));
		
			AddSubview(_MonsterView);

            _DieView = new DieRollerView();
            _DieView._CollpasedChanged += (object sender, EventArgs e) => 
            {
                LayoutSubviews();
            };
            AddSubview(_DieView);
			
			_CombatList.CombatState = state;
			
		}

		void HandlePlayerListCharacterSelectionChanged (object sender, CharacterSelectionChangedEventArgs e)
		{
			if (e.NewCharacter != null && e.NewCharacter != _SelectedCharacter)
			{
				SelectCharacter(e.NewCharacter);
			}
		}
		
		void SelectCharacter(Character ch)
		{
			if(ch != _SelectedCharacter)
			{
				_SelectedCharacter = ch;
				
				if (_SelectedCharacter != null)
				{
					_MonsterView.LoadHtmlString(MonsterHtmlCreator.CreateHtml(_SelectedCharacter.Monster, _SelectedCharacter), new NSUrl("http://localhost/"));
				}
			}
		}
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			
			CGRect rect = ConvertRectFromView(Frame, Superview);
			
			float margin = 2.0f;
			float width = 0;
			
			
            width = (float)(rect.Width / 4.0f);
			
			width -= margin;
			
			CGRect loc = new CGRect(0, 0, width, rect.Height);
			
			_CombatList.Frame = loc;
			
			loc.X += width + margin;
			_PlayerList.Frame = loc;
			
			loc.X += width + margin;
			_MonsterList.Frame = loc;
			
            if (_DieView.Collapsed)
            {
            
                loc.X += width + margin;
                loc.Height = Bounds.Height-_DieView.HiddenHeight;
                _MonsterView.Frame = loc;

                loc.Y = _MonsterView.Frame.Bottom;
                loc.Height = _DieView.HiddenHeight;
                _DieView.Frame = loc;
            }
            else
            {
                loc.X += width + margin;
                loc.Height = Bounds.Height*2.0f/5.0f;
                _MonsterView.Frame = loc;


                loc.Y = _MonsterView.Frame.Bottom;
                loc.Height = Bounds.Height - loc.Y;
                _DieView.Frame = loc;

            }
		}
	}
}

