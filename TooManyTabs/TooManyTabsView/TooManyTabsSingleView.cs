using Get.UI.Data;
using Get.Data.Properties;
using Get.Data.Bindings;
using Get.Data.Bindings.Linq;
namespace Gtudios.UI.TooManyTabs;
class TooManyTabsSingleView<T> : ContentBundleControl
{
    public Property<TooManyTabsSingleItem<T>> TooManyTabsSingleItemProperty { get; }
    public TooManyTabsSingleView(TooManyTabsView<T> Parent, TooManyTabsSingleItem<T> initial)
    {
        var ContentBundle = new ContentBundle<T, UIElement>();
        this.ContentBundle = ContentBundle;
        ContentBundle.ContentTemplateProperty.Bind(Parent.TabContentTemplateProperty, ReadOnlyBindingModes.OneWay);
        TooManyTabsSingleItemProperty = new(initial);
        ContentBundle.ContentProperty.Bind(TooManyTabsSingleItemProperty.SelectPath(x => x.ItemProperty), ReadOnlyBindingModes.OneWay);
    }
}