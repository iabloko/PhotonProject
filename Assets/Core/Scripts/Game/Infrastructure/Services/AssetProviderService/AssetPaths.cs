namespace Core.Scripts.Game.Infrastructure.Services.AssetProviderService
{
    public static class AssetPaths
    {
        #region NETWORKING

        public const string RECONNECTION_WINDOW_PATH = "Networking/Reconnection/ReconnectionView.prefab";

        public const string MATCHMAKING_PAGE = "Networking/Matchmaking/Page.prefab";

        public const string MATCHMAKING_ADAPTER = "Networking/Matchmaking/MatchmakingUIAdapter.prefab";
        public const string MATCHMAKING_SESSION = "Networking/Matchmaking/MatchmakingSession.prefab";

        public const string NETWORK_RUNNER = "NetworkingAssets/NetworkRunner";
        public const string NETWORK_SCENE_MANAGER = "NetworkingAssets/NetworkSceneManager";

        #endregion

        #region Player

        public const string PLAYER = "Player/MinimalCharacter";
        public const string PLAYER_CAMERA = "Cameras/CMVcamFollow";

        #endregion
        
        #region RESOURCES
        
        public const string FINISH = "FINISH";
        public const string SPEED_BOOSTER_ID = "SPEEDBOOSTER-PLANE";
        public const string LAVA_PLANE_ID = "LAVA-PLANE";
        public const string JUMP_PAD_ID = "JUMP-PAD";
        public const string CHECK_POINT_ID = "CHECK-POINT";
        public const string BUBBLEGUM_ID = "BUBBLEGUM-PLANE";
        public const string BANANA_ID = "BANANA-PLANE";
        public const string ASCII = "ASCIIArtPrefab";
        public const string PORTAL_SECOND_ID = "SecondPortal";
        public const string PORTAL_PRIMARY_ID = "PrimaryPortal";
        public const string INVISIBLE_LAVA_PLANE_ID = "INVISIBLE-LAVA-PLANE";
        public const string INVISIBLE_LAVA_LINE_ID = "INVISIBLE-LAVA-LINE";
        public const string INVISIBLE_WALL_ID = "INVISIBLE-WALL";
        
        #endregion
        
        #region PLAYER

        public const string PLAYER_DEATH_MESH = "Networking/PlayerDeathMesh.prefab";
        
        public const string PLAYER_VIRTUAL_CAMERA_FPS = "Camera/CM3rdNormalFps";
        public const string PLAYER_VIRTUAL_CAMERA_3_RD = "Camera/CM3rdNormal3rd";
        public const string PLAYER_VIRTUAL_CAMERA_PREVIEW = "Camera/CM3rdPreview";
        public const string PLAYER_VIRTUAL_CAMERA_TELEPORTATION = "Camera/CM3rdTeleportation";

        #endregion


        #region Scenes

        public const string TRANSITION_SCENE = "Transition";

        #endregion
    }
}