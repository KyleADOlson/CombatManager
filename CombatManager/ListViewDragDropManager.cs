/*
 *  ListViewDragDropManager.cs
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;
using WPF.JoshSmith.Adorners;
using WPF.JoshSmith.Controls.Utilities;
using System.Windows.Data;

namespace WPF.JoshSmith.ServiceProviders.UI
{

	/// <summary>
	/// Manages the dragging and dropping of ListBoxItems in a ListView.
	/// The ItemType type parameter indicates the type of the objects in
	/// the ListView's items source.  The ListView's ItemsSource must be 
	/// set to an instance of ObservableCollection of ItemType, or an 
	/// Exception will be thrown.
	/// </summary>
	/// <typeparam name="ItemType">The type of the ListView's items.</typeparam>
	public class ListViewDragDropManager<ItemType> where ItemType : class
	{

		bool canInitiateDrag;
		DragAdorner dragAdorner;
		double dragAdornerOpacity;
		int indexToSelect;
		bool isDragInProgress;
		ItemType itemUnderDragCursor;
		ListBox listView;
		Point ptMouseDown;
		bool showDragAdorner;

		/// <summary>
		/// Initializes a new instance of ListViewDragManager.
		/// </summary>
		public ListViewDragDropManager()
		{
			this.canInitiateDrag = false;
			this.dragAdornerOpacity = 0.7;
			this.indexToSelect = -1;
			this.showDragAdorner = false;
		}

		/// <summary>
		/// Initializes a new instance of ListViewDragManager.
		/// </summary>
		/// <param name="listView"></param>
        public ListViewDragDropManager(ListBox listView)
			: this()
		{
			this.ListView = listView;
		}

		/// <summary>
		/// Initializes a new instance of ListViewDragManager.
		/// </summary>
		/// <param name="listView"></param>
		/// <param name="dragAdornerOpacity"></param>
        public ListViewDragDropManager(ListBox listView, double dragAdornerOpacity)
			: this( listView )
		{
			this.DragAdornerOpacity = dragAdornerOpacity;
		}

		/// <summary>
		/// Initializes a new instance of ListViewDragManager.
		/// </summary>
		/// <param name="listView"></param>
		/// <param name="showDragAdorner"></param>
        public ListViewDragDropManager(ListBox listView, bool showDragAdorner)
			: this( listView )
		{
			this.ShowDragAdorner = showDragAdorner;
		}


		/// <summary>
		/// Gets/sets the opacity of the drag adorner.  This property has no
		/// effect if ShowDragAdorner is false. The default value is 0.7
		/// </summary>
		public double DragAdornerOpacity
		{
			get { return this.dragAdornerOpacity; }
			set
			{
				if( this.IsDragInProgress )
					throw new InvalidOperationException( "Cannot set the DragAdornerOpacity property during a drag operation." );

				if( value < 0.0 || value > 1.0 )
					throw new ArgumentOutOfRangeException( "DragAdornerOpacity", value, "Must be between 0 and 1." );

				this.dragAdornerOpacity = value;
			}
		}

		/// <summary>
		/// Returns true if there is currently a drag operation being managed.
		/// </summary>
		public bool IsDragInProgress
		{
			get { return this.isDragInProgress; }
			private set { this.isDragInProgress = value; }
		}


		/// <summary>
		/// Gets/sets the ListView whose dragging is managed.  This property
		/// can be set to null, to prevent drag management from occuring.  If
		/// the ListView's AllowDrop property is false, it will be set to true.
		/// </summary>
        public ListBox ListView
		{
			get { return listView; }
			set
			{
				if( this.IsDragInProgress )
					throw new InvalidOperationException( "Cannot set the ListView property during a drag operation." );

				if( this.listView != null )
				{
					this.listView.PreviewMouseLeftButtonDown -= listView_PreviewMouseLeftButtonDown;
					this.listView.PreviewMouseMove -= listView_PreviewMouseMove;
					this.listView.DragOver -= listView_DragOver;
					this.listView.DragLeave -= listView_DragLeave;
					this.listView.DragEnter -= listView_DragEnter;
					this.listView.Drop -= listView_Drop;

				}

				this.listView = value;

				if( this.listView != null )
				{
					if( !this.listView.AllowDrop )
						this.listView.AllowDrop = true;


					this.listView.PreviewMouseLeftButtonDown += listView_PreviewMouseLeftButtonDown;
					this.listView.PreviewMouseMove += listView_PreviewMouseMove;
					this.listView.DragOver += listView_DragOver;
					this.listView.DragLeave += listView_DragLeave;
					this.listView.DragEnter += listView_DragEnter;
					this.listView.Drop += listView_Drop;

				}
			}
		}

		/// <summary>
		/// Raised when a drop occurs.  By default the dropped item will be moved
		/// to the target index.  Handle this event if relocating the dropped item
		/// requires custom behavior.  Note, if this event is handled the default
		/// item dropping logic will not occur.
		/// </summary>
		public event EventHandler<ProcessDropEventArgs<ItemType>> ProcessDrop;


        /// <summary>
        /// Raised when a drop occurs.  By default the dropped item will be moved
        /// to the target index.  Handle this event if relocating the dropped item
        /// requires custom behavior.  Note, if this event is handled the default
        /// item dropping logic will not occur.
        /// </summary>
        public event EventHandler ManagerDragOver;


		/// <summary>
		/// Gets/sets whether a visual representation of the ListBoxItem being dragged
		/// follows the mouse cursor during a drag operation.  The default value is true.
		/// </summary>
		public bool ShowDragAdorner
		{
			get { return this.showDragAdorner; }
			set
			{
				if( this.IsDragInProgress )
					throw new InvalidOperationException( "Cannot set the ShowDragAdorner property during a drag operation." );

				this.showDragAdorner = value;
			}
		}

		void listView_PreviewMouseLeftButtonDown( object sender, MouseButtonEventArgs e )
		{
			if( this.IsMouseOverScrollbar )
			{
				// 4/13/2007 - Set the flag to false when cursor is over scrollbar.
				this.canInitiateDrag = false;
				return;
			}

			int index = this.IndexUnderDragCursor;
			this.canInitiateDrag = index > -1;

			if( this.canInitiateDrag )
			{
				// Remember the location and index of the ListBoxItem the user clicked on for later.
				this.ptMouseDown = MouseUtilities.GetMousePosition( this.listView );
				this.indexToSelect = index;
			}
			else
			{
				this.ptMouseDown = new Point( -10000, -10000 );
				this.indexToSelect = -1;
			}
		}

		void listView_PreviewMouseMove( object sender, MouseEventArgs e )
		{
			if( !this.CanStartDragOperation )
				return;

			// Select the item the user clicked on.
            if (this.listView.SelectedIndex != this.indexToSelect)
            {
                //this.listView.SelectedIndex = this.indexToSelect;
            }

			// If the item at the selected index is null, there's nothing
			// we can do, so just return;
			if( this.listView.SelectedItem == null )
				return;

			ListBoxItem itemToDrag = this.GetListBoxItem( this.listView.SelectedIndex );
			if( itemToDrag == null )
				return;

			AdornerLayer adornerLayer = this.ShowDragAdornerResolved ? this.InitializeAdornerLayer( itemToDrag ) : null;

			this.InitializeDragOperation( itemToDrag );
            try
            {
                this.PerformDragOperation();
                this.FinishDragOperation(itemToDrag, adornerLayer);
            }
            catch (InvalidOperationException)
            {

            }
		}


		void listView_DragOver( object sender, DragEventArgs e )
		{
			e.Effects = DragDropEffects.Move;

			if( this.ShowDragAdornerResolved )
				this.UpdateDragAdornerLocation();

			// Update the item which is known to be currently under the drag cursor.
			int index = this.IndexUnderDragCursor;
			this.ItemUnderDragCursor = index < 0 ? null : this.ListView.Items[index] as ItemType;

            if (ManagerDragOver != null)
            {
                ManagerDragOver(this, new EventArgs());
            }
		}


		void listView_DragLeave( object sender, DragEventArgs e )
		{
			if( !this.IsMouseOver( this.listView ) )
			{
				if( this.ItemUnderDragCursor != null )
					this.ItemUnderDragCursor = null;

				if( this.dragAdorner != null )
					this.dragAdorner.Visibility = Visibility.Collapsed;
			}
		}


		void listView_DragEnter( object sender, DragEventArgs e )
		{
			if( this.dragAdorner != null && this.dragAdorner.Visibility != Visibility.Visible )
			{
				// Update the location of the adorner and then show it.				
				this.UpdateDragAdornerLocation();
				this.dragAdorner.Visibility = Visibility.Visible;
			}
		}
	

		void listView_Drop( object sender, DragEventArgs e )
		{
			if( this.ItemUnderDragCursor != null )
				this.ItemUnderDragCursor = null;

			e.Effects = DragDropEffects.None;

			if( !e.Data.GetDataPresent( typeof( ItemType ) ) )
				return;

			// Get the data object which was dropped.
			ItemType data = e.Data.GetData( typeof( ItemType ) ) as ItemType;
			if( data == null )
				return;

            IList<ItemType> itemsSource = null;
			// Get the ObservableCollection<ItemType> which contains the dropped data object.
            if (this.listView.ItemsSource.GetType() == typeof(ListCollectionView))
            {
                itemsSource = ((ListCollectionView)this.listView.ItemsSource).SourceCollection as IList<ItemType>;
            }
            else
            {
                itemsSource = this.listView.ItemsSource as IList<ItemType>;
            }

			if( itemsSource == null )
				throw new Exception(
					"A ListView managed by ListViewDragManager must have its ItemsSource set to an ObservableCollection<ItemType>." );

			int oldIndex = itemsSource.IndexOf( data );
			int newIndex = this.IndexUnderDragCursor;

			if( newIndex < 0 )
			{
				// The drag started somewhere else, and our ListView is empty
				// so make the new item the first in the list.
				if( itemsSource.Count == 0 )
					newIndex = 0;

				// The drag started somewhere else, but our ListView has items
				// so make the new item the last in the list.
				else if( oldIndex < 0 )
					newIndex = itemsSource.Count;

				// The user is trying to drop an item from our ListView into
				// our ListView, but the mouse is not over an item, so don't
				// let them drop it.
				else
                {
                    newIndex = itemsSource.Count;
                };
			}

			if( this.ProcessDrop != null )
			{
				// Let the client code process the drop.
				ProcessDropEventArgs<ItemType> args = new ProcessDropEventArgs<ItemType>( itemsSource, data, oldIndex, newIndex, e.AllowedEffects );
				this.ProcessDrop( this, args );
				e.Effects = args.Effects;
			}
			else
			{
				// Move the dragged data object from it's original index to the
				// new index (according to where the mouse cursor is).  If it was
				// not previously in the ListBox, then insert the item.
                if (oldIndex > -1)
                {
                    itemsSource.RemoveAt(oldIndex);
                    itemsSource.Insert(newIndex, data);
                }
                else
                    itemsSource.Insert(newIndex, data);

				// Set the Effects property so that the call to DoDragDrop will return 'Move'.
				e.Effects = DragDropEffects.Move;
			}
		}

		bool CanStartDragOperation
		{
			get
			{
                if (Mouse.LeftButton != MouseButtonState.Pressed)
                {
                    return false;
                }
                if (!this.canInitiateDrag)
                {
                    return false;
                }

                if (this.indexToSelect == -1)
                {
                    return false;
                }

                if (!this.HasCursorLeftDragThreshold)
                {
                    return false;
                }

				return true;
			}
		}

		void FinishDragOperation( ListBoxItem draggedItem, AdornerLayer adornerLayer )
		{
			// Let the ListBoxItem know that it is not being dragged anymore.
			ListBoxItemDragState.SetIsBeingDragged( draggedItem, false );

			this.IsDragInProgress = false;

			if( this.ItemUnderDragCursor != null )
				this.ItemUnderDragCursor = null;

			// Remove the drag adorner from the adorner layer.
			if( adornerLayer != null )
			{
				adornerLayer.Remove( this.dragAdorner );
				this.dragAdorner = null;
			}
		}

		ListBoxItem GetListBoxItem( int index )
		{
			if( this.listView.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated )
				return null;

			return this.listView.ItemContainerGenerator.ContainerFromIndex( index ) as ListBoxItem;
		}

		ListBoxItem GetListBoxItem( ItemType dataItem )
		{
			if( this.listView.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated )
				return null;

			return this.listView.ItemContainerGenerator.ContainerFromItem( dataItem ) as ListBoxItem;
		}

		bool HasCursorLeftDragThreshold
		{
			get
			{
                if (this.indexToSelect < 0)
                {
                    return false;
                }

				ListBoxItem item = this.GetListBoxItem( this.indexToSelect );

                if (item == null)
                {
                    return false;
                }

				Rect bounds = VisualTreeHelper.GetDescendantBounds( item );
				Point ptInItem = this.listView.TranslatePoint( this.ptMouseDown, item );

				// In case the cursor is at the very top or bottom of the ListBoxItem
				// we want to make the vertical threshold very small so that dragging
				// over an adjacent item does not select it.
				double topOffset = Math.Abs( ptInItem.Y );
				double btmOffset = Math.Abs( bounds.Height - ptInItem.Y );
				double vertOffset = Math.Min( topOffset, btmOffset );

				double width = SystemParameters.MinimumHorizontalDragDistance * 2;
				double height = Math.Min( SystemParameters.MinimumVerticalDragDistance, vertOffset ) * 2;
				Size szThreshold = new Size( width, height );

				Rect rect = new Rect( this.ptMouseDown, szThreshold );
				rect.Offset( szThreshold.Width / -2, szThreshold.Height / -2 );
				Point ptInListView = MouseUtilities.GetMousePosition( this.listView );
				return !rect.Contains( ptInListView );
			}
		}


		/// <summary>
		/// Returns the index of the ListBoxItem underneath the
		/// drag cursor, or -1 if the cursor is not over an item.
		/// </summary>
		public int IndexUnderDragCursor
		{
			get
			{
				int index = -1;
				for( int i = 0; i < this.listView.Items.Count; ++i )
				{
					ListBoxItem item = this.GetListBoxItem( i );
					if( this.IsMouseOver( item ) )
					{
						index = i;
						break;
					}
				}
				return index;
			}
		}


		AdornerLayer InitializeAdornerLayer( ListBoxItem itemToDrag )
		{
			// Create a brush which will paint the ListBoxItem onto
			// a visual in the adorner layer.
			VisualBrush brush = new VisualBrush( itemToDrag );

			// Create an element which displays the source item while it is dragged.
			this.dragAdorner = new DragAdorner( this.listView, itemToDrag.RenderSize, brush );
			
			// Set the drag adorner's opacity.		
			this.dragAdorner.Opacity = this.DragAdornerOpacity;

			AdornerLayer layer = AdornerLayer.GetAdornerLayer( this.listView );
			layer.Add( dragAdorner );

			// Save the location of the cursor when the left mouse button was pressed.
			this.ptMouseDown = MouseUtilities.GetMousePosition( this.listView );

			return layer;
		}


		void InitializeDragOperation( ListBoxItem itemToDrag )
		{
			// Set some flags used during the drag operation.
			this.IsDragInProgress = true;
			this.canInitiateDrag = false;

			// Let the ListBoxItem know that it is being dragged.
			ListBoxItemDragState.SetIsBeingDragged( itemToDrag, true );
		}


		bool IsMouseOver( Visual target )
		{
			// We need to use MouseUtilities to figure out the cursor
			// coordinates because, during a drag-drop operation, the WPF
			// mechanisms for getting the coordinates behave strangely.
            if (target == null)
            {
                return false;
            }
			Rect bounds = VisualTreeHelper.GetDescendantBounds( target );
			Point mousePos = MouseUtilities.GetMousePosition( target );
			return bounds.Contains( mousePos );
		}


		/// <summary>
		/// Returns true if the mouse cursor is over a scrollbar in the ListView.
		/// </summary>
		bool IsMouseOverScrollbar
		{
			get
			{
				Point ptMouse = MouseUtilities.GetMousePosition( this.listView );
				HitTestResult res = VisualTreeHelper.HitTest( this.listView, ptMouse );
				if( res == null )
					return false;

				DependencyObject depObj = res.VisualHit;
				while( depObj != null )
				{
					if( depObj is ScrollBar  || depObj is TextBox)
						return true;

					// VisualTreeHelper works with objects of type Visual or Visual3D.
					// If the current object is not derived from Visual or Visual3D,
					// then use the LogicalTreeHelper to find the parent element.
					if( depObj is Visual || depObj is System.Windows.Media.Media3D.Visual3D )
						depObj = VisualTreeHelper.GetParent( depObj );
					else
						depObj = LogicalTreeHelper.GetParent( depObj );
				}

				return false;
			}
		}


		public ItemType ItemUnderDragCursor
		{
			get { return this.itemUnderDragCursor; }
			set
			{
				if( this.itemUnderDragCursor == value )
					return;

				// The first pass handles the previous item under the cursor.
				// The second pass handles the new one.
				for( int i = 0; i < 2; ++i )
				{
					if( i == 1 )
						this.itemUnderDragCursor = value;

					if( this.itemUnderDragCursor != null )
					{
						ListBoxItem ListBoxItem = this.GetListBoxItem( this.itemUnderDragCursor );
						if( ListBoxItem != null )
							ListBoxItemDragState.SetIsUnderDragCursor( ListBoxItem, i == 1 );
					}
				}
			}
		}

		void PerformDragOperation()
		{
			ItemType selectedItem = this.listView.SelectedItem as ItemType;
			DragDropEffects allowedEffects = DragDropEffects.Move | DragDropEffects.Move | DragDropEffects.Link;
			if( DragDrop.DoDragDrop( this.listView, selectedItem, allowedEffects ) != DragDropEffects.None )
			{
				// The item was dropped into a new location,
				// so make it the new selected item.
				this.listView.SelectedItem = selectedItem;
			}
		}


		bool ShowDragAdornerResolved
		{
			get { return this.ShowDragAdorner && this.DragAdornerOpacity > 0.0; }
		}

		void UpdateDragAdornerLocation()
		{
			if( this.dragAdorner != null )
			{
				Point ptCursor = MouseUtilities.GetMousePosition( this.ListView );

				double left = ptCursor.X - this.ptMouseDown.X;

				// 4/13/2007 - Made the top offset relative to the item being dragged.
				ListBoxItem itemBeingDragged = this.GetListBoxItem( this.indexToSelect );
				Point itemLoc = itemBeingDragged.TranslatePoint( new Point( 0, 0 ), this.ListView );
				double top = itemLoc.Y + ptCursor.Y - this.ptMouseDown.Y;

				this.dragAdorner.SetOffsets( left, top );
			}
		}

	}


	/// <summary>
	/// Exposes attached properties used in conjunction with the ListViewDragDropManager class.
	/// Those properties can be used to allow triggers to modify the appearance of ListBoxItems
	/// in a ListView during a drag-drop operation.
	/// </summary>
	public static class ListBoxItemDragState
	{
		/// <summary>
		/// Identifies the ListBoxItemDragState's IsBeingDragged attached property.  
		/// This field is read-only.
		/// </summary>
		public static readonly DependencyProperty IsBeingDraggedProperty =
			DependencyProperty.RegisterAttached(
				"IsBeingDragged",
				typeof( bool ),
				typeof( ListBoxItemDragState ),
				new UIPropertyMetadata( false ) );

		/// <summary>
		/// Returns true if the specified ListBoxItem is being dragged, else false.
		/// </summary>
		/// <param name="item">The ListBoxItem to check.</param>
		public static bool GetIsBeingDragged( ListBoxItem item )
		{
			return (bool)item.GetValue( IsBeingDraggedProperty );
		}

		/// <summary>
		/// Sets the IsBeingDragged attached property for the specified ListBoxItem.
		/// </summary>
		/// <param name="item">The ListBoxItem to set the property on.</param>
		/// <param name="value">Pass true if the element is being dragged, else false.</param>
		internal static void SetIsBeingDragged( ListBoxItem item, bool value )
		{
			item.SetValue( IsBeingDraggedProperty, value );
		}

		/// <summary>
		/// Identifies the ListBoxItemDragState's IsUnderDragCursor attached property.  
		/// This field is read-only.
		/// </summary>
		public static readonly DependencyProperty IsUnderDragCursorProperty =
			DependencyProperty.RegisterAttached(
				"IsUnderDragCursor",
				typeof( bool ),
				typeof( ListBoxItemDragState ),
				new UIPropertyMetadata( false ) );

		/// <summary>
		/// Returns true if the specified ListBoxItem is currently underneath the cursor 
		/// during a drag-drop operation, else false.
		/// </summary>
		/// <param name="item">The ListBoxItem to check.</param>
		public static bool GetIsUnderDragCursor( ListBoxItem item )
		{
			return (bool)item.GetValue( IsUnderDragCursorProperty );
		}

		/// <summary>
		/// Sets the IsUnderDragCursor attached property for the specified ListBoxItem.
		/// </summary>
		/// <param name="item">The ListBoxItem to set the property on.</param>
		/// <param name="value">Pass true if the element is underneath the drag cursor, else false.</param>
		internal static void SetIsUnderDragCursor( ListBoxItem item, bool value )
		{
			item.SetValue( IsUnderDragCursorProperty, value );
		}

	}



	/// <summary>
	/// Event arguments used by the ListViewDragDropManager.ProcessDrop event.
	/// </summary>
	/// <typeparam name="ItemType">The type of data object being dropped.</typeparam>
	public class ProcessDropEventArgs<ItemType> : EventArgs where ItemType : class
	{

        IList<ItemType> itemsSource;
		ItemType dataItem;
		int oldIndex;
		int newIndex;
		DragDropEffects allowedEffects = DragDropEffects.None;
		DragDropEffects effects = DragDropEffects.None;

		internal ProcessDropEventArgs(
            IList<ItemType> itemsSource, 
			ItemType dataItem, 
			int oldIndex, 
			int newIndex, 
			DragDropEffects allowedEffects )
		{
			this.itemsSource = itemsSource;
			this.dataItem = dataItem;
			this.oldIndex = oldIndex;
			this.newIndex = newIndex;
			this.allowedEffects = allowedEffects;
		}


		/// <summary>
		/// The items source of the ListView where the drop occurred.
		/// </summary>
        public IList<ItemType> ItemsSource
		{
			get { return this.itemsSource; }
		}

		/// <summary>
		/// The data object which was dropped.
		/// </summary>
		public ItemType DataItem
		{
			get { return this.dataItem; }
		}

		/// <summary>
		/// The current index of the data item being dropped, in the ItemsSource collection.
		/// </summary>
		public int OldIndex
		{
			get { return this.oldIndex; }
		}

		/// <summary>
		/// The target index of the data item being dropped, in the ItemsSource collection.
		/// </summary>
		public int NewIndex
		{
			get { return this.newIndex; }
		}

		/// <summary>
		/// The drag drop effects allowed to be performed.
		/// </summary>
		public DragDropEffects AllowedEffects
		{
			get { return allowedEffects; }
		}

		/// <summary>
		/// The drag drop effect(s) performed on the dropped item.
		/// </summary>
		public DragDropEffects Effects
		{
			get { return effects; }
			set { effects = value; }
		}

	}

}