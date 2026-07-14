public class Constants
{
	public static string STORY_PATH = "story/zh";
	public static string DEFAULT_STORY_FILE_NAME = "Test";
	public static string EXCEL_FILE_EXTENSION = ".xlsx";
	public static int DEFAULT_START_LINE = 1;

	public static string AVATAR_PATH = "image/avatar/";
	public static string BACKGROUND_PATH = "image/background/";
	public static string CHARACTER_PATH = "image/character/";
	public static string THUMBNAIL_PATH = "image/thumbnail/";
	public static string BUTTON_PATH = "image/button/";
	public static string VOCAL_PATH = "audio/vocal/";
	public static string MUSIC_PATH = "audio/music/";
	public static string AUDIO_LOAD_FAILED = "Failed to load audio: ";
	public static string IMAGE_LOAD_FAILED = "Failed to load Image: ";
	public static string BIG_IMAGE_LOAD_FAILED = "Failed to load big Image: ";
	public static string COORDINATE_MISSING = "Coornidate missing";

	public static string AUTO_ON = "autoplayon";
	public static string AUTO_OFF = "autoplayoff";
	public static float DEFAULT_AUTO_WAITING_SECONDS = 1f;

		public static string NO_DATA_FOUND = "No data found";
	public static string END_OF_STORY = "End of story";
	public static string CHOICE = "choice";
	public static string DEFAULT_STORY_NAME = "Test.xlsx";

	public static string APPEAR_AT = "appearAt";
	public static string APPEAR_AT_INSTANTLY = "appearAtInstantly";
	public static string DISAPPEAR = "disappear";
	public static string MOVE_TO = "moveTo";

	public static string SKIP_ON = "skipon";
	public static string SKIP_OFF = "skipoff";
	public static float DEFAULT_SKIP_WAITING_SECONDS = 0.02f;

	public static float DURATION_TIME = 1.0f;
	public static float DEFAULT_TYPING_SPEED = 0.05f;
	public static float SKIP_MODE_TYPING_SPEED = 0.01f;
	public static int MAX_LENGTH = 30;

	public static int DEFAULT_START_INDEX = 0;
	public static int SLOTS_PER_PAGE = 8;
	public static int TOTAL_SLOTS = 40;
	public static string COLON = ":";
	public static string CHOICE_SEPARATOR = "\n";
	public static string SAVE_GAME = "save_game";
	public static string SAVE_FILE_PATH = "saves";
	public static string SAVE_FILE_EXTENSION = ".json";
	public static string LOAD_GAME = "load_game";
	public static string EMPTY_SLOT = "empty_slot";

	public static int GALLERY_SLOTS_PER_PAGE =  9;
	public static string GALLERY = "gallery";
	public static string GALLERY_PLACEHOLDER = "gallery_placeholder";
	public static string[] ALL_BACKGROUNDS = {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10"};

	public static string UNLOCKED = "unlocked";

	public static string CONFIRM = "Confirm";
	public static string PROMPT_TEXT = "Please input your name: ";
	public static string PLAYER_NAME_TAG = "[Name]";

	public static string GOTO = "goto";

	public static string MASTER_VOLUME = "MasterVolume";
	public static string MUSIC_VOLUME = "MusicVolume";
	public static string VOICE_VOLUME = "VoiceVolume";
	public static float DEFAULT_VOLUME = 0.5f;
	public static string MENU_MUSIC_FILE_NAME = "1";
	
	public static string CREDITS_MUSIC_FILE_NAME = "1";

	public static string CREDITS_PATH = "credits";
	public static string CREDITS_FILE_EXTENSION = ".txt";
	public static string CREDITS_SCROLL_END = "Credits scrolling end";
	public static float CREDITS_SCROLL_SPEED = 100f;
	public static float CREDITS_SCROLL_END_Y = 4000f;
	public static string CREDITS_SCENE = "CreditsScene";
	// todo: Consider moving menu to a scene
	public static string MENU_SCENE = "SampleScene";
	public static string GAME_SCENE = "SampleScene";
}
