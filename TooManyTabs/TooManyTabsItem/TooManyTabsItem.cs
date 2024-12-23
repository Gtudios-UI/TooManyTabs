using Get.Data.Properties;
namespace Gtudios.UI.TooManyTabs;
[AutoProperty]
public abstract partial class TooManyTabsItem<T>
{
    public IProperty<TooManyTabsItem<T>?> ParentProperty { get; } = Auto<TooManyTabsItem<T>?>(null);
    public IProperty<ITooManyTabsStyle<T>> TabStyleProperty { get; }
    public TooManyTabsItem(ITooManyTabsStyle<T> style)
    {
        TabStyleProperty = Auto(style);
        TabStyleProperty.ApplyAndRegisterForNewValue((old, @new) =>
        {
            if (old is ITooManyTabsStyleOwner<T> oldOwner)
                oldOwner.Owner = null;
            if (@new is ITooManyTabsStyleOwner<T> newOwner)
                newOwner.Owner = this;
        });
    }
}