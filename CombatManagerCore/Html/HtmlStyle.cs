using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.Html
{
    public abstract class HtmlStyle
    {
        public abstract (string, string)[] ToArray();

        protected List<(string, string)> MakeList()
        {
            return new List<(string, string)>();
        }
    }

    public class HtmlSize : IEquatable<HtmlSize>
    {
        public const string Auto= "auto";
        public const string Inherit = "inherit";
        public const string Initial = "initial";
        public const string Medium = "medium";
        public const string Thick = "thin";
        public const string Thin = "thick";

        //absolute units
        public const string Percent = "%";
        public const string Picas = "pc";
        public const string Pixels = "px";
        public const string Points = "pt";
        public const string CM = "cm";
        public const string MM = "mm";
        public const string In = "in";

        //relative units
        public const string Element = "em";
        public const string XHeight = "ex";
        public const string CharWidth = "ch";
        public const string RootElement = "rem";
        public const string ViewWidth = "vw"; 
        public const string ViewHeight = "vh"; 
        public const string ViewMin = "vmin";
        public const string ViewMax = "vmax";

        public HtmlSize()
        {
            Type = Pixels;
            Value = 0;
            
        }

        public HtmlSize(HtmlSize size)
        {
            Type = size.Type;
            Value = size.Value;
        }

        public static HtmlSize AutoSize
        {
            get
            {
                return new HtmlSize(type: Auto);
            }
        }


        public static HtmlSize InheritSize
        {
            get
            {
                return new HtmlSize(type: Inherit);
            }
        }

        public static HtmlSize PixelSize(int size)
        {
            return new HtmlSize(value: size, type: Pixels);
        }
        public static HtmlSize PointSize(int size)
        {
            return new HtmlSize(value: size, type: Points);
        }
        public static HtmlSize CMSize(int size)
        {
            return new HtmlSize(value: size, type: CM);
        }
        public static HtmlSize PercentSize(int pct)
        {
            return new HtmlSize(value: pct, type: Percent);
        }


        public int Value { get; set; }
        public string Type { get; set; }

        public HtmlSize(int value = 0, string type = Pixels)
        {
            Value = value;
            Type = type;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case Auto:
                case Inherit:
                case Initial:
                    return Type;
                default:
                    return Value.ToString() + Type;
            }
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + (Type?.GetHashCode() ?? 0);
            hash = hash * 23 + Value;
            return hash;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            return Equals((HtmlSize)obj);

        }

        public bool Equals(HtmlSize other)
        {
            if (other == null)
            {
                return false;
            }
            return (other.Type == Type && other.Value == Value);
        }

        public static bool operator == (HtmlSize size1, HtmlSize size2)
        {
            if (((object)size1) == null)
            {
                return (((object)size2) == null);
            }

            return size1.Equals(size2);
        }

        public static bool operator !=(HtmlSize size1, HtmlSize size2)
        {
            return !size1.Equals(size2);
        }

    }

    public class GenericStyle : HtmlStyle
    {
        public string Name;
        public string Value;

        public GenericStyle() { }
        public GenericStyle(string name, string value) {
            Name = name;
            Value = value;
        }


        public override (string, string)[] ToArray()
        {
            return new []{(Name, Value)};
        }

    }
    public class SizeStyle : HtmlStyle
    {
        public HtmlSize _Width;
        public HtmlSize _Height;

        public HtmlSize Width
        {
            get
            {
                return _Width;
            }
            set
            {
                _Width = value.CopyDefault();
            }

        }
        public HtmlSize Height
        {
            get
            {
                return _Height;
            }
            set
            {
                _Height = value.CopyDefault();
            }

        }

        public SizeStyle(HtmlSize width, HtmlSize height)
        {
            this.Width = width.CopyDefault();
            this.Height = height.CopyDefault();

        }

        public override (string, string)[] ToArray()
        {
            var styles = MakeList();
            if (Width != null)
            {
                styles.AddSize("width", Width);
            }
            if (Height != null)
            {
                styles.AddSize("height", Height);
            }
            return styles.ToArray() ;
        }
    }


    public abstract class BoxStyle : HtmlStyle
    {
        HtmlSize _Top;
        HtmlSize _Bottom;
        HtmlSize _Left;
        HtmlSize _Right;


        public HtmlSize Top
        {
            get
            {
                return _Top;
            }
            set
            {
                _Top = value.CopyDefault();
            }
        }
        HtmlSize Bottom
        {
            get
            {
                return _Bottom;
            }
            set
            {
                _Bottom = value.CopyDefault();
            }
        }
        HtmlSize Left
        {
            get
            {
                return _Left;
            }
            set
            {
                _Left = value.CopyDefault();
            }
        }
        HtmlSize Right
        {
            get
            {
                return _Right;
            }
            set
            {
                _Right = value.CopyDefault();
            }
        }

        public enum BoxSide
        {
            Top,
            Right,
            Bottom,
            Left
        }

        public BoxStyle() { }

        public BoxStyle(HtmlSize size) : this (size, size, size, size)
        {
        }

        public BoxStyle(HtmlSize top, HtmlSize left, HtmlSize bottom, HtmlSize right)
        {
            Top = top;
            Left = left;
            Bottom = bottom;
            Right = right;
        }

        public BoxStyle(int top, int left, int bottom, int right, string type = HtmlSize.Pixels)
        {
            Top = new HtmlSize(top, type);
            Left = new HtmlSize(left, type);
            Bottom = new HtmlSize(bottom, type);
            Right = new HtmlSize(right, type);
        }

        public HtmlSize GetSide(BoxSide side)
        {
            switch (side)
            {
                case BoxSide.Top:
                    return Top;
                case BoxSide.Right:
                    return Right;
                case BoxSide.Bottom:
                    return Bottom;
                case BoxSide.Left:
                default:
                    return Left;
            }
        }

        public void SetSide(BoxSide side, HtmlSize val)
        {
            switch (side)
            {
                case BoxSide.Top:
                    Top = val;
                    break;
                case BoxSide.Right:
                    Right = val;
                    break;
                case BoxSide.Bottom:
                    Bottom = val;
                    break;
                case BoxSide.Left:
                default:
                    Left = val;
                    break;
            }
        }

        public void SetAllSides(HtmlSize size)
        {
            Action<BoxSide> b = x => SetSide(x, size);
            b.All();
        }

        public void SetSideValue(BoxSide side, int val)
        {
            switch (side)
            {
                case BoxSide.Top:
                    if (Top == null)
                    {
                        Top = new HtmlSize();
                    }
                    Top.Value = val;
                    break;
                case BoxSide.Right:
                    if (Right == null)
                    {
                        Right = new HtmlSize();
                    }
                    Right.Value = val;
                    break;
                case BoxSide.Bottom:
                    if (Bottom == null)
                    {
                        Bottom = new HtmlSize();
                    }
                    Bottom.Value = val;
                    break;
                case BoxSide.Left:
                default:
                    if (Left == null)
                    {
                        Left = new HtmlSize();
                    }
                    Left.Value = val;
                    break;
            }
        }


        public void SetAllSideValues(int value)
        {
            Action<BoxSide> b = x => SetSideValue(x, value);
            b.All();
        }

        public void SetSideType(BoxSide side, string type)
        {
            switch (side)
            {
                case BoxSide.Top:
                    if (Top == null)
                    {
                        Top = new HtmlSize();
                    }
                    Top.Type = type;
                    break;
                case BoxSide.Right:
                    if (Right == null)
                    {
                        Right = new HtmlSize();
                    }
                    Right.Type = type;
                    break;
                case BoxSide.Bottom:
                    if (Bottom == null)
                    {
                        Bottom = new HtmlSize();
                    }
                    Bottom.Type = type;
                    break;
                case BoxSide.Left:
                default:
                    if (Left == null)
                    {
                        Left = new HtmlSize();
                    }
                    Left.Type = type;
                    break;
            }
        }

        public void SetAllSideTypes(string type)
        {
            Action<BoxSide> b = x => SetSideType(x, type);
            b.All();
        }

        public abstract string Name
        {
            get;
        }

        public virtual string EqualStyle
        {
            get => Name;
        }
            

        public virtual string GetSidePostfix(BoxSide side)
        {
            switch (side)
            {
                case BoxSide.Top:
                    return "-top";
                case BoxSide.Right:
                    return "-right";
                case BoxSide.Bottom:
                    return "-bottom";
                case BoxSide.Left:
                default:
                    return "-left";
            }
        }

        public virtual string GetSideStyleName(BoxSide side)
        {
            return Name + GetSidePostfix(side);
        }

        public bool IsSidesEqual()
        {
            return (Top == Bottom) && (Top == Left) && (Top == Right);
        }

        public override (string, string)[] ToArray()
        {
            if (IsSidesEqual() && Top != null)
            { 
                return new[] { (EqualStyle, Top.ToString()) };
            }
            else
            {
                var list = MakeList();
                Action<BoxSide> b = (BoxSide x) =>
                {
                    if (GetSide(x) != null)
                    {
                        list.Add((GetSideStyleName(x), GetSide(x).ToString()));
                    }
                };
                b.All();
                     
                return list.ToArray();
            }
        }
    }

    public class MarginStyle : BoxStyle
    {
        public MarginStyle() : base() { }
        public MarginStyle(HtmlSize size) : base(size) { }
        public MarginStyle(HtmlSize top, HtmlSize left, HtmlSize bottom, HtmlSize right) : base(top, left, bottom, right) { }
        public MarginStyle(int top, int left, int bottom, int right) : base(top, left, bottom, right) { }
        public MarginStyle(int top, int left, int bottom, int right, string type) : base(top, left, bottom, right, type) { }

        public override string Name => "margin";
    }

    public class PaddingStyle : BoxStyle
    {
        public PaddingStyle() : base() { }
        public PaddingStyle(HtmlSize size) : base(size) { }
        public PaddingStyle(HtmlSize top, HtmlSize left, HtmlSize bottom, HtmlSize right) : base(top, left, bottom, right) { }
        public PaddingStyle(int top, int left, int bottom, int right) : base(top, left, bottom, right) { }
        public PaddingStyle(int top, int left, int bottom, int right, string type) : base(top, left, bottom, right, type) { }

        public override string Name => "padding";
    }

    public class BorderStyle : BoxStyle
    {
        public const string Dotted = "dotted";
        public const string Dashed = "dashed";
        public const string Solid = "solid";
        public const string Double = "double";
        public const string Groove = "groove";
        public const string Ridge = "ridge";
        public const string Inset = "inset";
        public const string Outset = "outset";
        public const string None = "none";
        public const string Hidden = "hidden";

        public const string DefaultStyle = Solid;



        public string TopStyle = DefaultStyle;
        public string BottomStyle = DefaultStyle;
        public string LeftStyle = DefaultStyle;
        public string RightStyle = DefaultStyle;


        public string Color { get; set; }


        public static string CombineBorders(string[] style)
        {
            string output = "";
            style.WeaveList(a => output += a, (a, b) => output += " ");
            return output;

        }


        public BorderStyle() : base()
        {
            SetAllSides(new HtmlSize(1));
        }
        public BorderStyle(HtmlSize size, string style = null) : this(size, size, size, size, style) { }
        public BorderStyle(HtmlSize top, HtmlSize left, HtmlSize bottom, HtmlSize right, string style = null) : base(top, left, bottom, right)
        {
            if (style != null)
            {
                SetAllBorderStyles(style);
            }
        }
        public BorderStyle(int top, int left, int bottom, int right, string type = HtmlSize.Pixels, string style = null) : base(top, left, bottom, right, type)
        {
            if (style != null)
            {
                SetAllBorderStyles(style);
            }
        }

        public override string Name => "border";

        public override string EqualStyle => Name + "-width";
        public bool IsStylesEqual()
        {
            return (TopStyle == BottomStyle) && (TopStyle == LeftStyle) && (TopStyle == RightStyle);
        }

        public virtual string GetSideBorderStyle(BoxSide side)
        {
            switch (side)
            {
                case BoxSide.Top:
                    return TopStyle;
                case BoxSide.Right:
                    return RightStyle;
                case BoxSide.Bottom:
                    return BottomStyle;
                case BoxSide.Left:
                default:
                    return LeftStyle;
            }
        }

        public virtual void SetSideBorderStyle(BoxSide side, string style)
        {
            switch (side)
            {
                case BoxSide.Top:
                    TopStyle = style;
                    break;
                case BoxSide.Right:
                    RightStyle = style;
                    break;
                case BoxSide.Bottom:
                    BottomStyle = style;
                    break;
                case BoxSide.Left:
                default:
                    LeftStyle = style;
                    break;
            }
        }

        public void SetAllBorderStyles(string style)
        {

            Action<BoxSide> b = (BoxSide x) => SetSideBorderStyle(x, style);
            b.All();
        }


        public override (string, string)[] ToArray()
        {
            var list = new List<(string, string)>(base.ToArray());
            if (IsStylesEqual() && TopStyle != null)
            {
                list.Add(("border-style", TopStyle));
            }
            else
            {

                Action<BoxSide> b = (BoxSide x) =>
                {
                    if (GetSideBorderStyle(x).NotNullString())
                    {
                        list.Add(("border-style" + GetSidePostfix(x), GetSideBorderStyle(x)));
                    }
                };
                b.All();
            }

            if (Color != null)
            {
                list.Add(("border-color", Color));
            }

            return list.ToArray();
        }
    }

    public static class HtmlStyleUtilities
    {
        public static List<(string, string)> AddSize(this List<(string, string)> list, string name, HtmlSize size)
        {
            list.Add((name, size.ToString()));
            return list;
        }

        public static (string, string)[] AddSize(this (string, string)[] list, string name, HtmlSize size)
        {
            return list.AppendEvenNull((name, size.ToString()));
        }

        public static HtmlSize CopyDefault(this HtmlSize size)
        {
            if (size == null)
            {
                return null;
            }
            return new HtmlSize(size);
        }

        public static string RGB(this (int, int, int) val)
        {
            return "rgb(" + val.ToStringList().WeaveString(", ") + ")";

        }
        public static string RGBA(this (int, int, int, decimal) val)
        {
            return "rgb(" + val.ToStringList().WeaveString(", ") + ")";

        }

        public static string HSL(this (int, int, int) val)
        {
            return "hsl(" + val.Item1 + ", " + val.Item2 + "%, " + val.Item3 + "%)";

        }
        public static string HSLA(this (int, int, int, decimal) val)
        {
            return "hsla(" + val.Item1 + ", " + val.Item2 + "%, " + val.Item3 + "%, " + val.Item4 + ")";

        }

        public static string HexColor(this (int, int, int) val)
        {
            return "#" + val.Item1.ToString("x2") + val.Item2.ToString("x2") + val.Item3.ToString("x2");

        }

        public static string HexColor(this (int, int, int, int) val)
        {
            return "#" + val.Item1.ToString("x2") + val.Item2.ToString("x2") + val.Item3.ToString("x2") + val.Item4.ToString("x2");

        }

        public static string HexColor24(this int val)
        {
            return "#" + val.ToString("x6");

        }

        public static string HexColor(this int val)
        {
            return "#" + val.ToString("x8");
        }

    }
        

}
