using Get.Data.Properties;
using Get.Data.Bindings;
using Get.Data.Bundles;
namespace Gtudios.UI.TooManyTabs;
[AutoProperty]
internal partial class TooManyTabsSingleView<T> : ContentBundleControl
{
    public IProperty<TooManyTabsSingleItem<T>> TooManyTabsSingleItemProperty { get; }
    public TooManyTabsSingleView(TooManyTabsView<T> Parent, TooManyTabsSingleItem<T> initial)
    {
        var ContentBundle = new ContentBundle<T, UIElement?>(default);
        this.ContentBundle = ContentBundle;
        ContentBundle.ContentTemplateBinding = OneWay(Parent.TabContentTemplateProperty);
        TooManyTabsSingleItemProperty = new Property<TooManyTabsSingleItem<T>>(initial);
        ContentBundle.ContentBinding = OneWay(TooManyTabsSingleItemProperty.SelectPath(x => x.ItemProperty));
    }
}