using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace CombatManagerDroid.Controls
{
    public class FlowLayout : ViewGroup
    {
        /**
         * Special value for the child view spacing.
         * SPACING_AUTO means that the actual spacing is calculated according to the size of the
         * container and the number of the child views, so that the child views are placed evenly in
         * the container.
         */
        public static int SPACING_AUTO = -65536;

        /**
         * Special value for the horizontal spacing of the child views in the last row
         * SPACING_ALIGN means that the horizontal spacing of the child views in the last row keeps
         * the same with the spacing used in the row above. If there is only one row, this value is
         * ignored and the spacing will be calculated according to childSpacing.
         */
        public static int SPACING_ALIGN = -65537;

        private static int SPACING_UNDEFINED = -65538;

        private static bool DEFAULT_FLOW = true;
        private static int DEFAULT_CHILD_SPACING = 0;
        private static int DEFAULT_CHILD_SPACING_FOR_LAST_ROW = SPACING_UNDEFINED;
        private static float DEFAULT_ROW_SPACING = 0;
        private static bool DEFAULT_RTL = false;
        private static int DEFAULT_MAX_ROWS = int.MaxValue;

        private bool mFlow = DEFAULT_FLOW;
        private int mChildSpacing = DEFAULT_CHILD_SPACING;
        private int mChildSpacingForLastRow = DEFAULT_CHILD_SPACING_FOR_LAST_ROW;
        private float mRowSpacing = DEFAULT_ROW_SPACING;
        private float mAdjustedRowSpacing = DEFAULT_ROW_SPACING;
        private bool mRtl = DEFAULT_RTL;
        private int mMaxRows = DEFAULT_MAX_ROWS;

        private List<float> mHorizontalSpacingForRow = new List<float>();
        private List<int> mHeightForRow = new List<int>();
        private List<int> mChildNumForRow = new List<int>();

        public FlowLayout(Context context) : this(context, null)
        {
        }

        public FlowLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {

            Android.Content.Res.TypedArray a = context.Theme.ObtainStyledAttributes(
                    attrs, Resource.Styleable.FlowLayout, 0, 0);
            try
            {
                mFlow = a.GetBoolean(Resource.Styleable.FlowLayout_flFlow, DEFAULT_FLOW);
                try
                {
                    mChildSpacing = a.GetInt(Resource.Styleable.FlowLayout_flChildSpacing, DEFAULT_CHILD_SPACING);
                }
                catch (Java.Lang.NumberFormatException e)
                {
                    mChildSpacing = a.GetDimensionPixelSize(Resource.Styleable.FlowLayout_flChildSpacing, (int)dpToPx(DEFAULT_CHILD_SPACING));
                }
                try
                {
                    mChildSpacingForLastRow = a.GetInt(Resource.Styleable.FlowLayout_flChildSpacingForLastRow, SPACING_UNDEFINED);
                }
                catch (Java.Lang.NumberFormatException e)
                {
                    mChildSpacingForLastRow = a.GetDimensionPixelSize(Resource.Styleable.FlowLayout_flChildSpacingForLastRow, (int)dpToPx(DEFAULT_CHILD_SPACING));
                }
                try
                {
                    mRowSpacing = a.GetInt(Resource.Styleable.FlowLayout_flRowSpacing, 0);
                }
                catch (Java.Lang.NumberFormatException e)
                {
                    mRowSpacing = a.GetDimension(Resource.Styleable.FlowLayout_flRowSpacing, dpToPx(DEFAULT_ROW_SPACING));
                }
                mMaxRows = a.GetInt(Resource.Styleable.FlowLayout_flMaxRows, DEFAULT_MAX_ROWS);
                mRtl = a.GetBoolean(Resource.Styleable.FlowLayout_flRtl, DEFAULT_RTL);
            }
            finally
            {
                a.Recycle();
            }
        }

        private float GetSpacingForRow(int spacingAttribute, int rowSize, int usedSize, int childNum)
        {
            float spacing;
            if (spacingAttribute == SPACING_AUTO)
            {
                if (childNum > 1)
                {
                    spacing = (rowSize - usedSize) / (childNum - 1);
                }
                else
                {
                    spacing = 0;
                }
            }
            else
            {
                spacing = spacingAttribute;
            }
            return spacing;
        }

        private float dpToPx(float dp)
        {
            return TypedValue.ApplyDimension(
                   ComplexUnitType.Dip, dp, Resources.DisplayMetrics);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            int widthSize = MeasureSpec.GetSize(widthMeasureSpec);
            int widthMode = (int)MeasureSpec.GetMode(widthMeasureSpec);
            int heightSize = MeasureSpec.GetSize(heightMeasureSpec);
            int heightMode = (int)MeasureSpec.GetMode(heightMeasureSpec);

            mHorizontalSpacingForRow.Clear();
            mChildNumForRow.Clear();
            mHeightForRow.Clear();

            int measuredHeight = 0, measuredWidth = 0, childCount = ChildCount;
            int rowWidth = 0, maxChildHeightInRow = 0, childNumInRow = 0;
            int rowSize = widthSize - PaddingLeft - PaddingRight;
            bool allowFlow = widthMode != 0 && mFlow;
            int childSpacing = mChildSpacing == SPACING_AUTO && widthMode == (int)MeasureSpecMode.Unspecified
                    ? 0 : mChildSpacing;
            float tmpSpacing = childSpacing == SPACING_AUTO ? (int)MeasureSpecMode.Unspecified : childSpacing;

            for (int i = 0; i < childCount; i++)
            {
                View child = GetChildAt(i);
                if (child.Visibility == ViewStates.Gone)
                {
                    continue;
                }

                LayoutParams childParams = child.LayoutParameters;
                int horizontalMargin = 0;
                int verticalMargin = 0;
                if (childParams is MarginLayoutParams)
                {
                    MeasureChildWithMargins(child, widthMeasureSpec, 0, heightMeasureSpec, measuredHeight);
                    MarginLayoutParams marginParams = (MarginLayoutParams)childParams;
                    horizontalMargin = marginParams.LeftMargin + marginParams.RightMargin;
                    verticalMargin = marginParams.TopMargin + marginParams.BottomMargin;
                }
                else
                {
                    MeasureChild(child, widthMeasureSpec, heightMeasureSpec);
                }

                int childWidth = child.MeasuredWidth + horizontalMargin;
                int childHeight = child.MeasuredHeight + verticalMargin;
                if (allowFlow && rowWidth + childWidth > rowSize)
                { // Need flow to next row
                  // Save parameters for current row
                    mHorizontalSpacingForRow.Add(
                            GetSpacingForRow(childSpacing, rowSize, rowWidth, childNumInRow));
                    mChildNumForRow.Add(childNumInRow);
                    mHeightForRow.Add(maxChildHeightInRow);
                    if (mHorizontalSpacingForRow.Count <= mMaxRows)
                    {
                        measuredHeight += maxChildHeightInRow;
                    }
                    measuredWidth = Math.Max(measuredWidth, rowWidth);

                    // Place the child view to next row
                    childNumInRow = 1;
                    rowWidth = childWidth + (int)tmpSpacing;
                    maxChildHeightInRow = childHeight;
                }
                else
                {
                    childNumInRow++;
                    rowWidth += (int)(childWidth + tmpSpacing);
                    maxChildHeightInRow = Math.Max(maxChildHeightInRow, childHeight);
                }
            }

            // Measure remaining child views in the last row
            if (mChildSpacingForLastRow == SPACING_ALIGN)
            {
                // For SPACING_ALIGN, use the same spacing from the row above if there is more than one
                // row.
                if (mHorizontalSpacingForRow.Count >= 1)
                {
                    mHorizontalSpacingForRow.Add(
                            mHorizontalSpacingForRow[mHorizontalSpacingForRow.Count - 1]);
                }
                else
                {
                    mHorizontalSpacingForRow.Add(
                            GetSpacingForRow(childSpacing, rowSize, rowWidth, childNumInRow));
                }
            }
            else if (mChildSpacingForLastRow != SPACING_UNDEFINED)
            {
                // For SPACING_AUTO and specific DP values, apply them to the spacing strategy.
                mHorizontalSpacingForRow.Add(
                        GetSpacingForRow(mChildSpacingForLastRow, rowSize, rowWidth, childNumInRow));
            }
            else
            {
                // For SPACING_UNDEFINED, apply childSpacing to the spacing strategy for the last row.
                mHorizontalSpacingForRow.Add(
                        GetSpacingForRow(childSpacing, rowSize, rowWidth, childNumInRow));
            }

            mChildNumForRow.Add(childNumInRow);
            mHeightForRow.Add(maxChildHeightInRow);
            if (mHorizontalSpacingForRow.Count <= mMaxRows)
            {
                measuredHeight += maxChildHeightInRow;
            }
            measuredWidth = Math.Max(measuredWidth, rowWidth);

            if (childSpacing == SPACING_AUTO)
            {
                measuredWidth = widthSize;
            }
            else if (widthMode == 0)
            {
                measuredWidth = measuredWidth + PaddingLeft + PaddingRight;
            }
            else
            {
                measuredWidth = Math.Min(measuredWidth + PaddingLeft + PaddingRight, widthSize);
            }

            measuredHeight += PaddingTop + PaddingBottom;
            int rowNum = Math.Min(mHorizontalSpacingForRow.Count, mMaxRows);

            float rowSpacing = (mRowSpacing == SPACING_AUTO && heightMode == (int)MeasureSpecMode.Unspecified) ? 0 : mRowSpacing;
            if (rowSpacing == SPACING_AUTO)
            {
                if (rowNum > 1)
                {
                    mAdjustedRowSpacing = (heightSize - measuredHeight) / (rowNum - 1);
                }
                else
                {
                    mAdjustedRowSpacing = 0;
                }
                measuredHeight = heightSize;
            }
            else
            {
                mAdjustedRowSpacing = rowSpacing;
                if (rowNum > 1)
                {
                    measuredHeight = heightMode == (int)MeasureSpecMode.Unspecified
                            ? ((int)(measuredHeight + mAdjustedRowSpacing * (rowNum - 1)))
                            : (Math.Min((int)(measuredHeight + mAdjustedRowSpacing * (rowNum - 1)),
                                        heightSize));
                }
            }

            measuredWidth = widthMode == (int)MeasureSpecMode.Exactly ? widthSize : measuredWidth;
            measuredHeight = heightMode == (int)MeasureSpecMode.Exactly ? heightSize : measuredHeight;
            SetMeasuredDimension(measuredWidth, measuredHeight);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            int paddingLeft = PaddingLeft;
            int paddingRight = PaddingRight;
            int paddingTop = PaddingTop;
            int x = mRtl ? (Width - paddingRight) : paddingLeft;
            int y = paddingTop;

            int rowCount = mChildNumForRow.Count, childIdx = 0;
            for (int row = 0; row < rowCount; row++)
            {
                int childNum = mChildNumForRow[row];
                int rowHeight = mHeightForRow[row];
                float spacing = mHorizontalSpacingForRow[row];
                for (int i = 0; i < childNum && childIdx < ChildCount;)
                {
                    View child = GetChildAt(childIdx++);
                    if (child.Visibility == ViewStates.Gone)
                    {
                        continue;
                    }
                    else
                    {
                        i++;
                    }

                    LayoutParams childParams = child.LayoutParameters;
                    int marginLeft = 0, marginTop = 0, marginRight = 0;
                    if (childParams is MarginLayoutParams)
                    {
                        MarginLayoutParams marginParams = (MarginLayoutParams)childParams;
                        marginLeft = marginParams.LeftMargin;
                        marginRight = marginParams.RightMargin;
                        marginTop = marginParams.TopMargin;
                    }

                    int childWidth = child.MeasuredWidth;
                    int childHeight = child.MeasuredHeight;
                    if (mRtl)
                    {
                        child.Layout(x - marginRight - childWidth, y + marginTop,
                                x - marginRight, y + marginTop + childHeight);
                        x -= (int)(childWidth + spacing + marginLeft + marginRight);
                    }
                    else
                    {
                        child.Layout(x + marginLeft, y + marginTop,
                                x + marginLeft + childWidth, y + marginTop + childHeight);
                        x += (int)(childWidth + spacing + marginLeft + marginRight);
                    }
                }
                x = mRtl ? (Width - paddingRight) : paddingLeft;
                y += (int)(rowHeight + mAdjustedRowSpacing);
            }
        }

        protected override LayoutParams GenerateLayoutParams(LayoutParams p)
        {
            return new MarginLayoutParams(p);
        }

        public override LayoutParams GenerateLayoutParams(IAttributeSet attrs)
        {
            return new MarginLayoutParams(Context, attrs);
        }

        /**
         * Returns whether to allow child views flow to next row when there is no enough space.
         *
         * @return Whether to flow child views to next row when there is no enough space.
         */
        public bool isFlow()
        {
            return mFlow;
        }

        /**
         * Sets whether to allow child views flow to next row when there is no enough space.
         *
         * @param flow true to allow flow. false to restrict all child views in one row.
         */
        public void setFlow(bool flow)
        {
            mFlow = flow;
            RequestLayout();
        }

        /**
         * Returns the horizontal spacing between child views.
         *
         * @return The spacing, either {@link FlowLayout#SPACING_AUTO}, or a fixed size in pixels.
         */
        public int GetChildSpacing()
        {
            return mChildSpacing;
        }

        /**
         * Sets the horizontal spacing between child views.
         *
         * @param childSpacing The spacing, either {@link FlowLayout#SPACING_AUTO}, or a fixed size in
         *        pixels.
         */
        public void setChildSpacing(int childSpacing)
        {
            mChildSpacing = childSpacing;
            RequestLayout();
        }

        /**
         * Returns the horizontal spacing between child views of the last row.
         *
         * @return The spacing, either {@link FlowLayout#SPACING_AUTO},
         *         {@link FlowLayout#SPACING_ALIGN}, or a fixed size in pixels
         */
        public int GetChildSpacingForLastRow()
        {
            return mChildSpacingForLastRow;
        }

        /**
         * Sets the horizontal spacing between child views of the last row.
         *
         * @param childSpacingForLastRow The spacing, either {@link FlowLayout#SPACING_AUTO},
         *        {@link FlowLayout#SPACING_ALIGN}, or a fixed size in pixels
         */
        public void setChildSpacingForLastRow(int childSpacingForLastRow)
        {
            mChildSpacingForLastRow = childSpacingForLastRow;
            RequestLayout();
        }

        /**
         * Returns the vertical spacing between rows.
         *
         * @return The spacing, either {@link FlowLayout#SPACING_AUTO}, or a fixed size in pixels.
         */
        public float GetRowSpacing()
        {
            return mRowSpacing;
        }

        /**
         * Sets the vertical spacing between rows in pixels. Use SPACING_AUTO to evenly place all rows
         * in vertical.
         *
         * @param rowSpacing The spacing, either {@link FlowLayout#SPACING_AUTO}, or a fixed size in
         *        pixels.
         */
        public void setRowSpacing(float rowSpacing)
        {
            mRowSpacing = rowSpacing;
            RequestLayout();
        }

        /**
         * Returns the maximum number of rows of the FlowLayout.
         *
         * @return The maximum number of rows.
         */
        public int GetMaxRows()
        {
            return mMaxRows;
        }

        /**
         * Sets the height of the FlowLayout to be at most maxRows tall.
         *
         * @param maxRows The maximum number of rows.
         */
        public void setMaxRows(int maxRows)
        {
            mMaxRows = maxRows;
            RequestLayout();
        }
    }
}
