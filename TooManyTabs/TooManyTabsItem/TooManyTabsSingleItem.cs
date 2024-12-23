using Get.Data.Properties;
namespace Gtudios.UI.TooManyTabs;
[AutoProperty]
public partial class TooManyTabsSingleItem<T>(T defaultValue, ITooManyTabsStyle<T> defaultStyle) : TooManyTabsItem<T>(defaultStyle)
{
    public IReadOnlyProperty<T> ItemProperty { get; } = AutoReadOnly(defaultValue);
}