using Get.Data.Properties;
using Get.Data.Bindings;
using Get.Data.Bundles;
using Windows.UI;

namespace Gtudios.UI.TooManyTabs;

using Get.Data.Collections;
using Get.Data.Collections.Update;
using Get.Data.Helpers;
using Get.UI.Controls.Panels;
using Platform.UI.Composition;
using System.Text;

[AutoProperty]
internal partial class TooManyTabsSingleView<T>(TooManyTabsView<T> parent, TooManyTabsSingleItem<T> initial) : TemplateControl<OrientedStack>, IConditionalMotionDragConnectionReceiver<TooManyTabsItem<T>>
{
    public IProperty<TooManyTabsSingleItem<T>> TooManyTabsSingleItemProperty { get; } = Auto(initial);

    public GlobalContainerRect GlobalRectangle => GlobalContainerRect.GetFromContainer(this);
    Compositor? compositor;
    SpriteVisual? overlayVisual;
    Vector3KeyFrameAnimation? scaleAnimation;
    Vector3KeyFrameAnimation? offsetAnimation;
    protected override void Initialize(OrientedStack rootElement)
    {
        parent.ConnectionContextProperty.ApplyAndRegisterForNewValue((old, @new) =>
        {
            if (old is not null)
                MotionDragConnectionContext<TooManyTabsItem<T>>.UnsafeRemove(old, this);
            if (@new is not null)
                MotionDragConnectionContext<TooManyTabsItem<T>>.UnsafeAdd(@new, this);
        });
        rootElement.Children.Add(
            new ContentBundleControl
            {
                ContentBundle = new ContentBundle<T, UIElement>(TooManyTabsSingleItem.Item)
                {
                    ContentBinding = OneWay(TooManyTabsSingleItemProperty.SelectPath(x => x.ItemProperty)),
                    ContentTemplateBinding = OneWay(parent.ToolbarTemplateProperty)
                }
            }.WithCustomCode(x => OrientedStack.LengthProperty.SetValue(x, Auto()))
        );
        rootElement.Children.Add(
            new ContentBundleControl
            {
                ContentBundle = new ContentBundle<T, UIElement>(TooManyTabsSingleItem.Item)
                {
                    ContentBinding = OneWay(TooManyTabsSingleItemProperty.SelectPath(x => x.ItemProperty)),
                    ContentTemplateBinding = OneWay(parent.TabContentTemplateProperty)
                }
            }.WithCustomCode(x => OrientedStack.LengthProperty.SetValue(x, Star()))
        );
        var gridVisual = ElementCompositionPreview.GetElementVisual(rootElement);
        compositor = gridVisual.Compositor;

        // Create a SpriteVisual with a semi-transparent white brush
        overlayVisual = compositor.CreateSpriteVisual();
        overlayVisual.Brush = compositor.CreateColorBrush(Color.FromArgb(51, 255, 255, 255)); // 0.2 opacity white
        overlayVisual.RelativeSizeAdjustment = new Vector2(1, 1);

        // Add the visual to the Composition Tree
        ElementCompositionPreview.SetElementChildVisual(rootElement, overlayVisual);

        // Initial state: hidden
        overlayVisual.Opacity = 0f;
    }

    public bool IsVisibleAt(Point pt)
    {
        SelfNote.HasDisallowedPInvoke();
        if (!(XamlRoot.IsHostVisible && Visibility is Visibility.Visible))
            return false;
        var ptScreen = GlobalRectangle.WindowPosOffset.Point() + pt;
        return WinWrapper.Windowing.Window.FromLocation(
            (int)ptScreen.X,
            (int)ptScreen.Y
        ).Root ==
        WinWrapper.Windowing.Window.FromWindowHandle(Windowing.Window.GetFromXamlRoot(XamlRoot).WindowHandle).Root;
    }
    bool isValidTarget;
    public new void DragEnter(object? sender, TooManyTabsItem<T> item, int senderIndex, DragPosition dragPosition, ref Point itemOffset)
    {
        isValidTarget = IsValidTarget(item);
        if (!isValidTarget) return;
        if (overlayVisual is not null)
            overlayVisual.Opacity = 1;
        Animate(ComputeDargPosition(dragPosition));
    }

    public void DragDelta(object? sender, TooManyTabsItem<T> item, int senderIndex, DragPosition dragPosition, ref Point itemOffset)
    {
        if (!isValidTarget) return;
        Animate(ComputeDargPosition(dragPosition));
    }
    public new void DragLeave(object? sender, TooManyTabsItem<T> item, int senderIndex)
    {
        if (!isValidTarget) return;
        if (overlayVisual is not null)
            overlayVisual.Opacity = 0;
    }

    public new void Drop(object? sender, TooManyTabsItem<T> item, int senderIndex, DragPosition dragPosition, DropManager dropManager)
    {
        if (!isValidTarget) return;
        var val = ComputeDargPosition(dragPosition);
        if (overlayVisual is not null)
            overlayVisual.Opacity = 0;
        if (val is not TooManyTabsSnapMode.None)
        {
            parent.OnTabSnapped(this, TooManyTabsSingleItem, item, val, dropManager);
        }
    }
    TooManyTabsSnapMode ComputeDargPosition(DragPosition dragPosition)
    {
        var pos = dragPosition.ToNewContainer(GlobalRectangle).MousePositionToContainer;
        var x = pos.X;
        var y = pos.Y;
        var w = ActualWidth;
        var h = ActualHeight;
        if (pos.X < w / 3)
        {
            if (y < h / 3 && y < x)
                return TooManyTabsSnapMode.Top;
            if (y > h * 2 / 3 && h - y < x)
                return TooManyTabsSnapMode.Bottom;
            return TooManyTabsSnapMode.Left;
        }
        else if (x > w * 2 / 3)
        {
            if (y < h / 3 && y < w - x)
                return TooManyTabsSnapMode.Top;
            if (y > h * 2 / 3 && h - y < w - x)
                return TooManyTabsSnapMode.Bottom;
            return TooManyTabsSnapMode.Right;
        }
        else if (y < h / 3)
        {
            return TooManyTabsSnapMode.Top;
        }
        else if (y > h * 2 / 3)
        {
            return TooManyTabsSnapMode.Bottom;
        }
        else
        {
            return TooManyTabsSnapMode.Center;
        }
    }
    bool IsValidTarget(TooManyTabsItem<T> item)
    {
        var cur = TooManyTabsSingleItem;
        while (item != null)
        {
            if (item == cur)
            {
                return false;
            }
            if (item.Parent == null)
            {
                return true;
            }
            item = item.Parent;
        }

        return false;
    }
    TooManyTabsSnapMode prevsnapMode = TooManyTabsSnapMode.None;
    void Animate(TooManyTabsSnapMode snapMode)
    {
        if (prevsnapMode == snapMode)
            return;
        if (overlayVisual is not null)
        {
            scaleAnimation = compositor!.CreateVector3KeyFrameAnimation();
            scaleAnimation.Duration = TimeSpan.FromMilliseconds(100);
            scaleAnimation.InsertKeyFrame(1f, new(
                x: snapMode is TooManyTabsSnapMode.Left or TooManyTabsSnapMode.Right ? 0.5f : 1,
                y: snapMode is TooManyTabsSnapMode.Top or TooManyTabsSnapMode.Bottom ? 0.5f : 1,
                z: 1
            ));
            offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(100);
            offsetAnimation.InsertKeyFrame(1f, new(
                x: snapMode is TooManyTabsSnapMode.Right ? 0.5f : 0,
                y: snapMode is TooManyTabsSnapMode.Bottom ? 0.5f : 0,
                z: 0
            ));
            overlayVisual.StartAnimation(nameof(overlayVisual.Scale), scaleAnimation);
            overlayVisual.StartAnimation(nameof(overlayVisual.RelativeOffsetAdjustment), offsetAnimation);
        }
    }

    public bool Accepts(object? sender, TooManyTabsItem<T> item, int senderIndex)
    {
        return parent.IsTabSnapEnabled && IsValidTarget(item);
    }
}
static class Extension
{
    public static DV Size(this Rect r) => new(r.Width, r.Height);
    public static Size ToSize(this Rect r) => new(r.Width, r.Height);
    public static Point ToPoint(this Rect r) => new(r.X, r.Y);
    public static DV Point(this Rect r) => new(r.X, r.Y);
    public static DV Point(this Point p) => p;
    public static DV Size(this Size s) => s;
}

readonly record struct DV(double X, double Y)
{
    public Point Point => new(X, Y);
    public Size Size => new(X, Y);

    public static DV operator +(DV a, double b)
        => new(a.X + b, a.Y + b);
    public static DV operator +(DV a, DV b)
        => new(a.X + b.X, a.Y + b.Y);
    public static DV operator -(DV a)
        => new(-a.X, -a.Y);
    public static DV operator -(DV a, DV b)
        => a + -b;
    public static implicit operator Point(DV p) => p.Point;
    public static implicit operator Size(DV s) => s.Size;
    public static implicit operator DV(Point p) => new(p.X, p.Y);
    public static implicit operator DV(Size s) => new(s.Width, s.Height);
    public static implicit operator Vector3(DV p) => new((float)p.X, (float)p.Y, 0);
}
public enum TooManyTabsSnapMode
{
    None,
    Top,
    Left,
    Right,
    Bottom,
    Center
}