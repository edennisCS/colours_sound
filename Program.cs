using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading; // For Thread.Sleep

public class Program
{
    // Define a smaller range of colors
    private static readonly List<KnownColor> BasicColors = new List<KnownColor>
    {
        KnownColor.Black,
        KnownColor.White,
        KnownColor.Red,
        KnownColor.Green,
        KnownColor.Blue,
        KnownColor.Yellow,
        KnownColor.Cyan,
        KnownColor.Magenta,
        KnownColor.Gray,
        KnownColor.DarkGray,
        KnownColor.LightGray
    };

    // Define a mapping from known color names to sound frequencies (Hz)
    private static readonly Dictionary<KnownColor, int> ColorSoundMap = new Dictionary<KnownColor, int>
    {
        { KnownColor.Black, 261 },   // C4
        { KnownColor.White, 293 },   // D4
        { KnownColor.Red, 329 },     // E4
        { KnownColor.Green, 349 },   // F4
        { KnownColor.Blue, 391 },    // G4
        { KnownColor.Yellow, 440 },  // A4
        { KnownColor.Cyan, 493 },    // B4
        { KnownColor.Magenta, 523 }, // C5
        { KnownColor.Gray, 587 },    // D5
        { KnownColor.DarkGray, 659 },// E5
        { KnownColor.LightGray, 698 } // F5
    };

    public static void Main()
    {
        // Test color example: R=128, G=130, B=135
        Color testColor = Color.FromArgb(128, 130, 135);

        // First, test the custom color
        ProcessColor(testColor);

        // Then iterate over each color in BasicColors
        foreach (var knownColor in BasicColors)
        {
            Color color = Color.FromKnownColor(knownColor);
            ProcessColor(color);
        }
    }

    public static void ProcessColor(Color color)
    {
        // Determine closest known color
        KnownColor closestKnownColor = GetClosestKnownColor(color);
        Color closestColor = Color.FromKnownColor(closestKnownColor);

        if (ColorSoundMap.TryGetValue(closestKnownColor, out int frequency))
        {
            Console.WriteLine($"Color RGB ({color.R}, {color.G}, {color.B}) " +
                              $"is closest to {closestKnownColor} with RGB ({closestColor.R}, {closestColor.G}, {closestColor.B})");

            // Determine dominant RGB component(s)
            List<string> dominantComponents = GetDominantRGBComponents(color);
            Console.WriteLine($"Playing sound for frequency: {frequency} Hz with emphasis on {string.Join(", ", dominantComponents)} component(s).");

            // Play the sound with frequency modulation based on the dominant RGB component(s)
            PlayGlockenspielSound(frequency, color);
        }
        else
        {
            Console.WriteLine($"No sound mapping exists for color {closestKnownColor}.");
        }
        Thread.Sleep(1500); // Delay to distinguish between sounds
    }

    public static List<string> GetDominantRGBComponents(Color color)
    {
        int totalRGB = color.R + color.G + color.B;
        var dominantComponents = new List<string>();

        // Calculate the percentage of each RGB component
        double redPercentage = (double)color.R / totalRGB * 100;
        double greenPercentage = (double)color.G / totalRGB * 100;
        double bluePercentage = (double)color.B / totalRGB * 100;

        // Define thresholds for dominance
        double dominanceThreshold = 30.0; // Example threshold for dominance in percentage

        if (redPercentage > dominanceThreshold) dominantComponents.Add("Red");
        if (greenPercentage > dominanceThreshold) dominantComponents.Add("Green");
        if (bluePercentage > dominanceThreshold) dominantComponents.Add("Blue");

        return dominantComponents;
    }

    public static void PlayGlockenspielSound(int frequency, Color color)
    {
        int beepDuration = 400; // Duration of each individual beep (ms)
        int waveDuration = 300; // Total duration of the wave effect (ms)
        int waveCycles = 1; // Number of up-and-down cycles
        double maxWaveAmplitude = 20; // Maximum frequency modulation amplitude
        int stepDuration = 50; // Duration of each step in the modulation
        int numSteps = waveDuration / stepDuration;

        // Get dominant RGB components
        var dominantComponents = GetDominantRGBComponents(color);

        for (int cycle = 0; cycle < waveCycles; cycle++)
        {
            for (int step = 0; step < numSteps; step++)
            {
                // Start with the base frequency
                double modFrequency = frequency;

                // Apply easing function to modulation amplitude
                double easingFactor = EasingFunction(step, numSteps);

                // Apply modulation based on dominant components with eased amplitude
                if (dominantComponents.Contains("Red"))
                {
                    modFrequency += maxWaveAmplitude * easingFactor * Math.Sin((2 * Math.PI * 1 * step) / numSteps);
                }
                if (dominantComponents.Contains("Green"))
                {
                    modFrequency += maxWaveAmplitude * easingFactor * Math.Sin((2 * Math.PI * 2 * step) / numSteps);
                }
                if (dominantComponents.Contains("Blue"))
                {
                    modFrequency += maxWaveAmplitude * easingFactor * Math.Sin((2 * Math.PI * 3 * step) / numSteps);
                }

                // Convert modFrequency to an integer for Console.Beep
                int frequencyToPlay = (int)Math.Round(modFrequency);

                // Ensure frequency is within the valid range for Console.Beep
                frequencyToPlay = Math.Max(37, Math.Min(frequencyToPlay, 32767));

                // Play the beep with the modulated frequency
                Console.Beep(frequencyToPlay, beepDuration);
            }
        }

        // Add a pause to separate the next note
        Thread.Sleep(500); // Delay after each glockenspiel note
    }

    public static double EasingFunction(int step, int totalSteps)
    {
        double t = (double)step / (totalSteps - 1); // Normalized time

        // Ease-In-Out function: Smooth transition in and out
        if (t < 0.5)
        {
            return 8 * t * t * t * t;
        }
        else
        {
            double f = (t - 1);
            return 1 - 8 * f * f * f * f;
        }
    }


    public static KnownColor GetClosestKnownColor(Color color)
    {
        KnownColor closestColorName = KnownColor.Transparent;
        double minDistance = double.MaxValue;

        foreach (KnownColor knownColor in BasicColors)
        {
            Color known = Color.FromKnownColor(knownColor);
            double distance = GetColorDistance(color, known);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestColorName = knownColor;
            }
        }

        return closestColorName;
    }

    public static double GetColorDistance(Color c1, Color c2)
    {
        // Calculate the Euclidean distance between two colors
        int rDifference = c1.R - c2.R;
        int gDifference = c1.G - c2.G;
        int bDifference = c1.B - c2.B;
        return Math.Sqrt(rDifference * rDifference + gDifference * gDifference + bDifference * bDifference);
    }
}
