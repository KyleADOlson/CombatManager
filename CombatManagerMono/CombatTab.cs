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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CombatManager;
using System.Drawing;

namespace CombatManagerMono
{
	public class CombatTab: CMTab
	{
		CombatListView combatListView;
		CharacterListView playerList;
		CharacterListView monsterList;
		UIWebView monsterView;
		
		Character _SelectedCharacter;
		
		
		public CombatTab (CombatState state) : base (state)
		{
			combatListView = new CombatListView();
			//
			
			AddSubview(combatListView);
			playerList = new CharacterListView(CombatState, false);
			playerList.CharacterSelectionChanged += HandlePlayerListCharacterSelectionChanged;
			AddSubview(playerList);
			monsterList = new CharacterListView(CombatState, true);
			monsterList.CharacterSelectionChanged += HandlePlayerListCharacterSelectionChanged;
			AddSubview(monsterList);
			
			monsterView = new UIWebView(new RectangleF(0, 0, 100, 100));
			monsterView.BackgroundColor = UIColor.Brown;
			monsterView.LoadHtmlString("<html></html>", new NSUrl("http://localhost/"));
		
			AddSubview(monsterView);
			
			combatListView.CombatState = state;
			
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
					monsterView.LoadHtmlString(MonsterHtmlCreator.CreateHtml(_SelectedCharacter.Monster, _SelectedCharacter), new NSUrl("http://localhost/"));
				}
			}
		}
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			
			RectangleF rect = ConvertRectFromView(Frame, Superview);
			
			float margin = 2.0f;
			float width = 0;
			
			
			if (UIExtensions.IsVertical)
			{
				width = rect.Width / 3.0f;
				
			}
			else
			{
				width = rect.Width / 4.0f;
			}
			width -= margin;
			
			RectangleF loc = new RectangleF(0, 0, width, rect.Height);
			
			combatListView.Frame = loc;
			
			loc.X += width + margin;
			playerList.Frame = loc;
			
			loc.X += width + margin;
			monsterList.Frame = loc;
			
			if (!UIExtensions.IsVertical)
			{
				loc.X += width + margin;
				monsterView.Frame = loc;
			}
			
			
			
			
		}
	}
}

