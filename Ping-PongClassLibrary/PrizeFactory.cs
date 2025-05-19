namespace Ping_PongClassLibrary
{
    /// <summary>
    /// Абстрактный базовый класс, реализующий паттерн Фабричный метод для создания призов в игре.
    /// </summary>
    public abstract class PrizeFactory
    {
        /// <summary>
        /// Создает новый приз с заданными параметрами.
        /// </summary>
        public abstract IPrize CreatePrize(double x, double y, int textureId, double spawnTime);
    }
}