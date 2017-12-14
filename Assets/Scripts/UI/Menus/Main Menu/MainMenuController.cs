using InstaDungeon.MapGeneration;
using InstaDungeon.Settings;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace InstaDungeon.UI
{
	public class MainMenuController : MonoBehaviour
	{
		private static readonly string DefaultLayoutGeneratorPath = "Settings/DefaultHilbertLayoutDefinition";
		private static readonly string DefaultZoneGeneratorPath = "Settings/DefaultCavernousZoneDefinition";

		[SerializeField] private Dropdown layoutDropdown;
		[SerializeField] private Dropdown zoneDropdown;
		[SerializeField] private Toggle enableCustomSeed;
		[SerializeField] private InputField customSeedField;
		[SerializeField] private Dropdown modeDropdown;
		[SerializeField] private Button startButton;
		[SerializeField] private Button quitButton;

		private ScriptingManager scriptingManager;

		#region [MonoBehaviour Methods]

		private void Awake()
		{
			scriptingManager = Locator.Get<ScriptingManager>();
			enableCustomSeed.onValueChanged.AddListener(ChangeEnableCustomSeed);
			startButton.onClick.AddListener(RequestStart);
			quitButton.onClick.AddListener(RequestQuit);
		}

		private void Start()
		{
			PopulateLayoutDropdown(layoutDropdown);
			PopulateZoneDropdown(zoneDropdown);
			customSeedField.interactable = enableCustomSeed.isOn;
		}

		#endregion

		#region [Events]

		private void ChangeEnableCustomSeed(bool value)
		{
			customSeedField.interactable = value;
		}

		private void RequestStart()
		{
			ILayoutGenerator layoutGenerator = GetLayoutGenerator();
			IZoneGenerator zoneGenerator = GetZoneGenerator();
			int seed = GetSeed();
			ControlMode controlMode = GetControlMode();

			GameSettings settings = new GameSettings(layoutGenerator, zoneGenerator, seed, controlMode);
			Locator.Get<GameFeederManager>().Settings = settings;
			SceneManager.LoadScene("Game");
		}

		private void RequestQuit()
		{
			Application.Quit();
		}

		#endregion

		private void PopulateLayoutDropdown(Dropdown dropdown)
		{
			dropdown.ClearOptions();

			List<string> options = new List<string>()
			{
				"Hilbert"
			};

			options.AddRange(scriptingManager.GetLayoutGeneratorNames());

			dropdown.AddOptions(options);
		}

		private void PopulateZoneDropdown(Dropdown dropdown)
		{
			dropdown.ClearOptions();

			List<string> options = new List<string>()
			{
				"Cavernous"
			};

			options.AddRange(scriptingManager.GetZoneGeneratorNames());

			dropdown.AddOptions(options);
		}

		private ILayoutGenerator GetLayoutGenerator()
		{
			ILayoutGenerator result;

			if (layoutDropdown.value == 0)
			{
				HilbertLayoutGeneratorDefinition layoutGeneratorDefinition = Resources.Load<HilbertLayoutGeneratorDefinition>(DefaultLayoutGeneratorPath);
				result = new HilbertLayoutGenerator(layoutGeneratorDefinition.Settings);
			}
			else
			{
				List<string> scriptGenerators = scriptingManager.GetLayoutGeneratorNames();
				result = scriptingManager.GetLayoutGenerator(scriptGenerators[layoutDropdown.value - 1]);
			}

			return result;
		}

		private IZoneGenerator GetZoneGenerator()
		{
			IZoneGenerator result;

			if (zoneDropdown.value == 0)
			{
				CavernousZoneGeneratorDefinition zoneGeneratorDefinition = Resources.Load<CavernousZoneGeneratorDefinition>(DefaultZoneGeneratorPath);
				result = new CavernousZoneGenerator(zoneGeneratorDefinition.Settings);
			}
			else
			{
				List<string> scriptGenerators = scriptingManager.GetZoneGeneratorNames();
				result = scriptingManager.GetZoneGenerator(scriptGenerators[zoneDropdown.value - 1]);
			}

			return result;
		}

		private int GetSeed()
		{
			int result;

			if (enableCustomSeed.isOn == false || int.TryParse(customSeedField.text, out result) == false)
			{
				result = System.Guid.NewGuid().GetHashCode() ^ System.DateTime.UtcNow.Millisecond;
			}

			return result;
		}

		private ControlMode GetControlMode()
		{
			ControlMode result;

			switch (modeDropdown.value)
			{
				default:
				case 0: result = ControlMode.Manual; break;
				case 1: result = ControlMode.Auto; break;
			}

			return result;
		}
	}
}
