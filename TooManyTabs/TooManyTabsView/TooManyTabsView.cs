using Get.UI.Data;
using Get.Data.Collections;
using Get.Data.Properties;
using Get.Data.DataTemplates;
using Get.Data.Bindings.Linq;
using Gtudios.UI.MotionDrag;
namespace Gtudios.UI.TooManyTabs;
[AutoProperty]
public partial class TooManyTabsView<T> : TemplateControl<UserControl>
{
    public IProperty<IDataTemplate<T, UIElement>?> TabHeaderTemplateProperty { get; } = Auto<IDataTemplate<T, UIElement>?>(default);
    public IProperty<IDataTemplate<T, UIElement>?> TabContentTemplateProperty { get; } = Auto<IDataTemplate<T, UIElement>?>(default);
    public IProperty<TooManyTabsMultiItem<T>> TooManyTabsItemProperty { get; } = Auto<TooManyTabsMultiItem<T>>(new());
    public IProperty<MotionDragConnectionContext<TooManyTabsItem<T>>> ConnectionContextProperty { get; } = Auto<MotionDragConnectionContext<TooManyTabsItem<T>>>(new());
    protected override void Initialize(UserControl rootElement)
    {
        rootElement.Content =
            new TooManyTabsViewSelector<T>(
                this,
                TooManyTabsItemProperty.Select<TooManyTabsMultiItem<T>, TooManyTabsItem<T>>(x => x)
            );
    }
}