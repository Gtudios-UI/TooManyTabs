using Get.Data.Collections.Update;
using Get.Data.Collections;
using Get.Data.Properties;
using Get.Data.UIModels;
using Get.Data.Collections.Linq;
namespace Gtudios.UI.TooManyTabs;
[AutoProperty]
public partial class TooManyTabsMultiItem<T> : TooManyTabsItem<T>
{
    public TooManyTabsMultiItem(ITooManyTabsStyle<T> defaultStyle) : base(defaultStyle)
    {
        ParentSetter<T> p = new(this);
        ChildrenSelectionManagerProperty.Select(x => x.Collection).ApplyAndRegisterForNewValue((old, @new) =>
        {
            old.ItemsChanged -= p.ItemsChangedHandler;
            @new.ItemsChanged += p.ItemsChangedHandler;
        });
    }

    public IProperty<SelectionManagerMutable<TooManyTabsItem<T>>>
        ChildrenSelectionManagerProperty { get; } =
        Auto(new SelectionManagerMutable<TooManyTabsItem<T>>());
    public IUpdateCollection<TooManyTabsItem<T>> Tabs => ChildrenSelectionManager.Collection;
    public IProperty<Orientation> TabViewOrientationProperty { get; } = Auto(Orientation.Horizontal);
}
readonly struct ParentSetter<T>(TooManyTabsItem<T> parent)
{
    public void ItemsChangedHandler(IEnumerable<IUpdateAction<TooManyTabsItem<T>>> actions)
    {
        foreach (var act in actions)
        {
            switch (act)
            {
                case ItemsAddedUpdateAction<TooManyTabsItem<T>> added:
                    foreach (var item in added.Items.AsEnumerable())
                    {
                        ItemAdded(item);
                    }
                    break;
                case ItemsRemovedUpdateAction<TooManyTabsItem<T>> removed:
                    foreach (var item in removed.Items.AsEnumerable())
                    {
                        ItemRemoved(item);
                    }
                    break;
                case ItemsReplacedUpdateAction<TooManyTabsItem<T>> replaced:
                    ItemRemoved(replaced.OldItem);
                    ItemAdded(replaced.NewItem);
                    break;
            }
        }
    }
    void ItemAdded(TooManyTabsItem<T> item)
    {
        if (item.Parent is not null)
            throw new InvalidOperationException("The item must not have any parents");
        else
            item.Parent = parent;
    }
    void ItemRemoved(TooManyTabsItem<T> item)
    {
        if (item.Parent == parent)
            item.Parent = null;
    }

}