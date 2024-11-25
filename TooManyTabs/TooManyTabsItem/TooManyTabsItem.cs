using Get.Data.Properties;
namespace Gtudios.UI.TooManyTabs;
[AutoProperty]
public abstract partial class TooManyTabsItem<T>
{
    public IProperty<string> NameProperty { get; } = Auto("");
}