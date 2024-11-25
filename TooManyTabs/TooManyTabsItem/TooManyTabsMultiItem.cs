using Get.Data.Collections.Update;
using Get.Data.Collections;
using Get.Data.Properties;
using Get.Data.UIModels;
namespace Gtudios.UI.TooManyTabs;
[AutoProperty]
public partial class TooManyTabsMultiItem<T> : TooManyTabsItem<T>
{
    public IProperty<SelectionManagerMutable<TooManyTabsItem<T>>>
        ChildrenSelectionManagerProperty { get; } =
        Auto(new SelectionManagerMutable<TooManyTabsItem<T>>());
    public IUpdateCollection<TooManyTabsItem<T>> Tabs => ChildrenSelectionManager.Collection;
}