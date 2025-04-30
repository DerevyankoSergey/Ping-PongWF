using System;

namespace Ping_PongClassLibrary
{
    public class RandomPrizeFactory : PrizeFactory
    {
        private readonly double width = 40;
        private readonly double height = 40;

        public override IPrize CreatePrize(double x, double y, int textureId, double spawnTime)
        {
            // Тип приза определяется на основе textureId
            switch (textureId)
            {
                case 4: // prize_length.png теперь увеличивает ширину
                    return new IncreaseWidthPrize(x, y, width, height, textureId, spawnTime);
                case 5: // prize_width.png теперь увеличивает длину
                    return new IncreaseLengthPrize(x, y, width, height, textureId, spawnTime);
                case 6: // prize_material.png остаётся без изменений
                    return new ChangeMaterialPrize(x, y, width, height, textureId, spawnTime);
                default:
                    throw new InvalidOperationException($"Unknown textureId: {textureId}");
            }
        }
    }
}