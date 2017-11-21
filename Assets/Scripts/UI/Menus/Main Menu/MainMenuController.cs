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

		#region [MonoBehaviour Methods]

		private void Awake()
		{
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

			dropdown.AddOptions(options);
		}

		private void PopulateZoneDropdown(Dropdown dropdown)
		{
			dropdown.ClearOptions();

			List<string> options = new List<string>()
			{
				"Cavernous"
			};

			dropdown.AddOptions(options);
		}

		private ILayoutGenerator GetLayoutGenerator()
		{
			ILayoutGenerator result;

			switch (layoutDropdown.value)
			{
				default:
				case 0:
					HilbertLayoutGeneratorDefinition layoutGeneratorDefinition = Resources.Load<HilbertLayoutGeneratorDefinition>(DefaultLayoutGeneratorPath);
					result = new HilbertLayoutGenerator(layoutGeneratorDefinition.Settings);
					break;
			}

			return result;
		}

		private IZoneGenerator GetZoneGenerator()
		{
			IZoneGenerator result;

			switch (zoneDropdown.value)
			{
				default:
				case 0:
					CavernousZoneGeneratorDefinition zoneGeneratorDefinition = Resources.Load<CavernousZoneGeneratorDefinition>(DefaultZoneGeneratorPath);
					result = new CavernousZoneGenerator(zoneGeneratorDefinition.Settings);
					break;
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
