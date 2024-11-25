using Get.Data.Properties;
namespace Gtudios.UI.TooManyTabs;
[AutoProperty]
public partial class TooManyTabsSingleItem<T>(T defaultValue) : TooManyTabsItem<T>
{
    public IProperty<T> ItemProperty { get; } = Auto(defaultValue);
}