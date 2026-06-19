using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Interfaces;

namespace GymManagementSystem.BLL.Services;

public class AiAssistantService : IAiAssistantService
{
    private static readonly Dictionary<string, string> Responses = new(StringComparer.OrdinalIgnoreCase)
    {
        ["hello"] = "Hi there! Welcome to Power Fitness. How can I help you today?",
        ["hi"] = "Hello! How can I assist you with your gym experience?",
        ["hours"] = "We're open Mon-Fri 6AM-10PM, Sat 7AM-8PM, and Sun 8AM-6PM.",
        ["opening hours"] = "Mon-Fri: 6AM-10PM | Sat: 7AM-8PM | Sun: 8AM-6PM",
        ["price"] = "Our plans start at $29/month for Basic, $49/month for Standard, and $79/month for Premium.",
        ["pricing"] = "Plans: Basic $29/mo, Standard $49/mo, Premium $79/mo. Each with increasing perks!",
        ["membership"] = "We have 3 membership tiers. Visit the Plans section to see full details.",
        ["plans"] = "Basic ($29): Gym access | Standard ($49): Gym + Classes | Premium ($79): All access + PT sessions",
        ["trainer"] = "We have certified personal trainers available. Visit the Trainers page to view profiles.",
       ["personal trainer"] = "Our trainers specialize in strength, cardio, yoga, and more. Book a session today!",
        ["class"] = "We offer yoga, pilates, spinning, HIIT, and strength training classes. Check the Sessions page!",
        ["yoga"] = "Our yoga classes run Mon/Wed/Fri at 7AM and Tue/Thu at 6PM. Zen awaits!",
        ["hiit"] = "HIIT sessions are Mon/Wed/Fri at 8AM and Tue/Thu at 5PM. Bring your energy!",
        ["spinning"] = "Spinning classes: daily at 6AM and 6PM. Book your bike!",
        ["location"] = "We're located at 123 Fitness Street, downtown. Come visit us!",
        ["parking"] = "Free parking is available for members in the rear lot.",
        ["pool"] = "Yes! Our Premium members have access to the rooftop pool.",
        ["sauna"] = "Sauna access is included with Standard and Premium memberships.",
        ["shower"] = "Locker rooms with showers and changing areas are available for all members.",
        ["free trial"] = "Yes! We offer a 3-day free trial. Sign up to claim yours!",
        ["trial"] = "New members can enjoy a 3-day free trial with full access.",
        ["cancel"] = "You can cancel your membership anytime from your Dashboard or contact admin.",
        ["refund"] = "Refunds are available within 14 days of purchase. Contact admin for details.",
        ["contact"] = "You can reach us at support@powerfitness.com or call (555) 123-4567.",
        ["email"] = "Our support email is support@powerfitness.com.",
        ["phone"] = "Call us at (555) 123-4567 during business hours.",
        ["diet"] = "We have a nutritionist on site for consultations. Book a session through the Dashboard.",
        ["nutrition"] = "Our nutritionist offers personalized meal plans. Ask at the front desk!",
        ["water"] = "Water fountains and refill stations are available throughout the gym.",
        ["wifi"] = "Free Wi-Fi is available for all members. Password: powerfit2024",
        ["age"] = "Members must be at least 14 years old. Minors need parental consent.",
        ["guest"] = "Members can bring a guest for $10 per visit. Premium members get 2 free guest passes/month.",
        ["help"] = "I can answer questions about: hours, pricing, memberships, classes, trainers, location, facilities, and more!"
    };

    private static readonly string[] Fallbacks =
    {
        "Great question! For more details, please contact our team at support@powerfitness.com.",
        "I'm not sure about that one. Try asking about hours, pricing, or classes!",
        "That's beyond my knowledge. Our staff at the front desk would be happy to help!",
        "Hmm, I don't have that info yet. Feel free to ask something else!"
    };

    public Task<string> GetResponseAsync(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return Task.FromResult("Please ask me something!");

        foreach (var kvp in Responses)
        {
            if (message.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(kvp.Value);
        }

        var random = new Random();
        return Task.FromResult(Fallbacks[random.Next(Fallbacks.Length)]);
    }
}
