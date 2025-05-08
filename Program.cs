using Reflection2;

class Program
{
    static void Main(string[] args)
    {
        MySettings settings = new MySettings();

        // Load settings
        settings.LoadSettings();
        Console.WriteLine($"Plugin_SettingOne: {settings.SettingOne}");
        Console.WriteLine($"Plugin_SettingTwo: {settings.SettingTwo}");

        // Modify settings
        settings.SettingOne = 42;
        settings.SettingTwo = "Hello, World!";

        Console.WriteLine($"Plugin_SettingOne Modified: {settings.SettingOne}");
        Console.WriteLine($"Plugin_SettingTwo Modified: {settings.SettingTwo}");

        // Save settings
        settings.SaveSettings();
        Console.WriteLine("Settings saved.");
    }
}