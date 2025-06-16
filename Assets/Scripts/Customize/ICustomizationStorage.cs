public interface ICustomizationStorage
{
    void Save(CustomizationData data);
    CustomizationData Load();
    bool HasData();
}