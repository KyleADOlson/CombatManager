/*
 *  MainUI.cs
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
using System.Threading;

namespace CombatManagerMono
{
	public class MainUI : UIView
	{
		
		ToolbarView toolbar;
		CMTab currentTab;
		
		CombatTab _combatTab;
		MonstersTab _monstersTab;
		SpellsTab _spellsTab;
		FeatsTab _featsTab;
		RulesTab _rulesTab;
		
		static MainUI _MainView;
		
        static object _Lock = new object();
		static CombatState _CombatState = new CombatState();
		
		public MainUI ()
		{
			_MainView = this;
			
			BackgroundColor = UIExtensions.RGBColor(0x0);
			
			toolbar = new ToolbarView();
			
			currentTab = new CombatTab(_CombatState);

			
			AddSubview(toolbar);
			AddSubview(currentTab);
			
		}
		
        private static CombatState _NextCombatState = null;

		public static void SaveCombatState()
		{
            DateTime startTime = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("SaveCombatState");
            try
            {
                CombatState s = new CombatState(_CombatState);


                Thread t = new Thread(delegate() {

                  try
                    {
                        XmlLoader<CombatState>.Save(s, "CombatState.xml", true);


                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Failure saving combat state: " + ex.ToString());
                    }

                });
                t.Start ();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failure starting save combat state: " + ex.ToString());
            }

            System.Diagnostics.Debug.WriteLine("Finished SaveCombatState Time: " + 
                (DateTime.Now - startTime).TotalSeconds.ToString() + " secs");

		}
		
		public static void LoadCombatState()
		{            
			try
            {
				CombatState state = XmlLoader<CombatState>.Load("CombatState.xml", true);
				if (state != null)
				{
					_CombatState.Copy(state);
					_CombatState.SortCombatList(false, false, true);

					_CombatState.FixInitiativeLinksList(new List<Character>(_CombatState.Characters));
				}

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failure loading combat state: " + ex.ToString());
			}
			
			if (_CombatState == null)
			{
			
				_CombatState = new CombatState();	
			}
			_CombatState.CharacterAdded += Handle_CombatStateCharacterAdded;
			_CombatState.CharacterRemoved += Handle_CombatStateCharacterRemoved;
		}

		static void Handle_CombatStateCharacterRemoved (object sender, CombatStateCharacterEventArgs e)
		{
			//SaveCombatState();
		}

		static void Handle_CombatStateCharacterAdded (object sender, CombatStateCharacterEventArgs e)
		{
			SaveCombatState();
		}
		
		public override void LayoutSubviews ()
		{
			Resize();
			
		}
		
		private void Resize()
		{
			
			base.LayoutSubviews ();
			toolbar.SetWidth(Frame.Width);
			toolbar.SetHeight(50);
			toolbar.SetLocation(0, 0);
			toolbar.ButtonClicked += HandleToolbarButtonClicked;
			
			
			currentTab.SetLocation(0, toolbar.Frame.Height);
			currentTab.SetWidth(Frame.Width);
			currentTab.SetHeight(Frame.Height - toolbar.Frame.Height);


		}

		void HandleToolbarButtonClicked (object sender, int button)
		{
			CMTab newTab = null;
			switch (button)
			{
			case 0:
				if (!(currentTab is CombatTab))
				{
					newTab = CombatTab;	
				}
				break;
			case 1:
				if (!(currentTab is MonstersTab))
				{
					newTab = MonstersTab;
				}
				break;
			case 2:
				if (!(currentTab is FeatsTab))
				{
					newTab = FeatsTab;	
				}
				break;
			case 3:
				if (!(currentTab is SpellsTab))
				{
					newTab = SpellsTab;	
				}
				break;
			case 4:
				if (!(currentTab is RulesTab))
				{
					newTab = RulesTab;	
				}
				break;
			}
			
			if (newTab !=null && newTab != currentTab)
			{
				currentTab.RemoveFromSuperview();
				currentTab = newTab;
				AddSubview(newTab);
				Resize();
				
			}
			
		}
		
		CombatTab CombatTab
		{
			get
			{
				if (_combatTab == null)
				{
					_combatTab = new CombatTab(_CombatState);
				}
				return _combatTab;
			}
		}
		
		MonstersTab MonstersTab
		{
			get
			{
				if (_monstersTab == null)
				{
					_monstersTab = new MonstersTab(_CombatState);
				}
				return _monstersTab;
			}
		}
		
		SpellsTab SpellsTab
		{
			get
			{
				if (_spellsTab == null)
				{
					_spellsTab = new SpellsTab(_CombatState);
				}
				return _spellsTab;
			}
		}
		
		FeatsTab FeatsTab
		{
			get
			{
				if (_featsTab == null)
				{
					_featsTab = new FeatsTab(_CombatState);
				}
				return _featsTab;
			}
		}
		
		RulesTab RulesTab
		{
			get
			{
				if (_rulesTab == null)
				{
					_rulesTab = new RulesTab(_CombatState);
				}
				return _rulesTab;
			}
		}
		
		public static MainUI MainView
		{
			get
			{
				return _MainView;
			}
		}
	}
}

