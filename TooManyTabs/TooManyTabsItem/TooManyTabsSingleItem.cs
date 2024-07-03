using Get.Data.Properties;
namespace Gtudios.UI.TooManyTabs;
public class TooManyTabsSingleItem<T> : TooManyTabsItem<T>
{
    public Property<T> ItemProperty { get; } = new(default);
    public T Item { get => ItemProperty.Value; set => ItemProperty.Value = value; }
}