namespace Core.Scripts.Game.Infrastructure.ModelData.InteractionObjects
{
    public class EffectDuration
    {
        public const float BANANA_DURATION = 2.5f;
        public const float SPEED_UP_BOOST_DURATION = 3.0f;
        public const float BUBBLEGUM_SLOWDOWN_DURATION = 3.0f;
    }

    public enum InteractionObjectType
    {
        NONE = -1,
        CHECK_POINT = 0,
        LAVA_PLANE = 1,
        INVISIBLE_LAVA_PLANE = 2,
        INVISIBLE_WALL = 3,
        FINISH = 4,
        JUMP_PAD = 5,
        BALL = 6,
        CONTROLLED_CUBE = 7,
        BANANA = 8,
        BUBBLE_GUM = 9,
        SPEED_BOOSTER_PLANE = 10,
        PRIMARY_PORTAL = 11,
        SECOND_PORTAL = 12,
        EXIT_PORTAL = 13,
        INVISIBLE_LAVA_LANE = 14,
        DEATH_PLANE = 15,
    }
}