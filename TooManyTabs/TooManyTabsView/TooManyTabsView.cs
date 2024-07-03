using Get.UI.Data;
using Get.Data.Collections;
using Get.Data.Properties;
using Get.Data.DataTemplates;
using Get.Data.Bindings.Linq;
namespace Gtudios.UI.TooManyTabs;
public class TooManyTabsView<T> : TemplateControl<UserControl>
{
    public Property<IDataTemplate<T, UIElement>?> TabHeaderTemplateProperty { get; } = new(default);
    public Property<IDataTemplate<T, UIElement>?> TabContentTemplateProperty { get; } = new(default);
    public Property<TooManyTabsMultiItem<T>> TooManyTabsItemProperty { get; } = new(new());
    protected override void Initialize(UserControl rootElement)
    {
        rootElement.Content =
            new TooManyTabsViewSelector<T>(
                this,
                TooManyTabsItemProperty.Select<TooManyTabsMultiItem<T>, TooManyTabsItem<T>>(x => x)
            );
    }
}