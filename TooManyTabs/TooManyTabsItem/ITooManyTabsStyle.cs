namespace Gtudios.UI.TooManyTabs;
public interface ITooManyTabsStyle<T>;
public interface ITooManyTabsStyleOwner<T> : ITooManyTabsStyle<T>
{
    TooManyTabsItem<T>? Owner { set; }
}