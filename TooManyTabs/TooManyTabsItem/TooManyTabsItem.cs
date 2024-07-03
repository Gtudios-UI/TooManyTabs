using Get.Data.Properties;
namespace Gtudios.UI.TooManyTabs;
public abstract class TooManyTabsItem<T>
{
    public Property<string> NameProperty { get; } = new("");
    public string Name { get => NameProperty.Value; set => NameProperty.Value = value; }
}