using CharlotteMod.Content.Cards;
using CharlotteMod.Content.Mechanics;

namespace Sts2Mod.Foreseer;

internal static class CharlotteForecasts
{
    public static void Subscribe(string modId)
        => ForeseerCardAdapter.Subscribe(modId, card => card is MultipleExhibits or Masterstroke, Play);

    private static void Play(ForecastPlay forecast)
    {
        if (forecast.Card is MultipleExhibits)
        {
            forecast.Attack(forecast.Count("Cards"));
            return;
        }
        forecast.ExhaustAndAttack(News.IsNews);
    }
}
