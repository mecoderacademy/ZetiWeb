using System.Text.RegularExpressions;

namespace Zeti.Models;

public class Vehicle
{
    public string Vin { get; set; }
    private string _licensePlate;

    public string LicensePlate
    {
        get => _licensePlate;
        set
        {
            _licensePlate = SanitizeLicensePlate(value);
        }
    }

    public string Make { get; set; }
    public string Model { get; set; }
    public State State { get; set; }


  

    private string SanitizeLicensePlate(string value)
    {
        // Remove any characters that are not alphanumeric or space
        value = value.Substring(0, Math.Min(value.Length, 9));
        string sanitizedValue = Regex.Replace(value, "[^a-zA-Z0-9 ]", "");
        // Ensure the format "^[a-zA-Z0-9]+ [a-zA-Z0-9]+$" and truncate to exactly 9 characters
        if (!Regex.IsMatch(sanitizedValue, "^[a-zA-Z0-9]+ [a-zA-Z0-9]+$"))
        {
            // If the format is not correct, you can handle it accordingly.
            // For example, you can choose to throw an exception or modify the value to fit the format.
            throw new ArgumentException("Invalid license plate format");
        }

        // Truncate to exactly 9 characters
       

        return sanitizedValue;
    }
}

