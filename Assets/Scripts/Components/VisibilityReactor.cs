using InstaDungeon.Events;
using UnityEngine;

namespace InstaDungeon.Components
{
	[RequireComponent(typeof(Entity))]
	public class VisibilityReactor : MonoBehaviour
	{
		private static readonly Color32 LerpColor = new Color32(26, 21, 32, 255);
		private static readonly float ObscuredAmount = 1f;
		private static readonly float PreviouslySeenAmount = 0.5f;
		private static readonly float VisibleAmount = 0f;

		private Entity entity;
		private MapManager mapManager;
		private SpriteRenderer[] renderers;
		private int colorId;
		private int lerpAmountId;

		private Cell currentCell;

		private void Reset()
		{
			// renderers = GetComponentsInChildren<SpriteRenderer>(true);
			// InitializeRenderers(renderers);
		}

		private void Awake()
		{
			renderers = GetComponentsInChildren<SpriteRenderer>(true);
			entity = GetComponent<Entity>();
			mapManager = Locator.Get<MapManager>();
			colorId = Shader.PropertyToID("_Color");
			lerpAmountId = Shader.PropertyToID("_Amount");
		}

		private void OnEnable()
		{
			SubscribeEvents();
		}

		private void OnDisable()
		{
			UnsubscribeEvents();
		}

		private void SubscribeEvents()
		{
			SubscribeEntityEvents();
		}

		private void SubscribeEntityEvents()
		{
			entity.Events.AddListener(OnEntityAddedToMap, EntityAddToMapEvent.EVENT_TYPE);
			entity.Events.AddListener(OnEntityRelocate, EntityRelocateEvent.EVENT_TYPE);
			entity.Events.AddListener(OnEntityFinishMoving, EntityFinishMovementEvent.EVENT_TYPE);
		}

		private void UnsubscribeEvents()
		{
			UnsubscribeEntityEvents();
			UnsubscribeCellEvents();
		}

		private void UnsubscribeEntityEvents()
		{
			entity.Events.RemoveListener(OnEntityAddedToMap, EntityAddToMapEvent.EVENT_TYPE);
			entity.Events.RemoveListener(OnEntityRelocate, EntityRelocateEvent.EVENT_TYPE);
			entity.Events.RemoveListener(OnEntityFinishMoving, EntityFinishMovementEvent.EVENT_TYPE);
		}

		private void UnsubscribeCellEvents()
		{
			if (currentCell != null)
			{
				currentCell.Events.RemoveListener(OnCellVisibilityChanges, CellVisibilityEventData.EVENT_TYPE);
			}
		}

		private void OnEntityAddedToMap(IEventData eventData)
		{
			UnsubscribeCellEvents();
			currentCell = mapManager.Map[entity.CellTransform.Position];

			if (currentCell != null)
			{
				RefreshVisibility();
				currentCell.Events.AddListener(OnCellVisibilityChanges, CellVisibilityEventData.EVENT_TYPE);
			}
		}

		private void OnEntityRelocate(IEventData eventData)
		{
			UnsubscribeCellEvents();
			currentCell = mapManager.Map[entity.CellTransform.Position];

			if (currentCell != null)
			{
				RefreshVisibility();
			}
		}

		private void OnEntityFinishMoving(IEventData eventData)
		{
			UnsubscribeCellEvents();
			currentCell = mapManager.Map[entity.CellTransform.Position];

			if (currentCell != null)
			{
				RefreshVisibility();
				currentCell.Events.AddListener(OnCellVisibilityChanges, CellVisibilityEventData.EVENT_TYPE);
			}
		}

		private void OnCellVisibilityChanges(IEventData eventData)
		{
			RefreshVisibility();
		}

		private void InitializeRenderers(SpriteRenderer[] renderers)
		{
			if (renderers != null)
			{
				Shader colorLerpShader = Shader.Find("Sprites/DefaultColorLerp");
				int pixelSnapId = Shader.PropertyToID("PixelSnap");

				for (int i = 0; i < renderers.Length; i++)
				{
					renderers[i].sharedMaterial.shader = colorLerpShader;
					renderers[i].sharedMaterial.SetFloat(pixelSnapId, 1f);
				}
			}
		}

		private void RefreshVisibility()
		{
			if (currentCell != null && renderers != null)
			{
				for (int i = 0; i < renderers.Length; i++)
				{
					UpdateRenderer(renderers[i], currentCell.Visibility);
				}
			}
		}

		private void UpdateRenderer(SpriteRenderer renderer, VisibilityType visibility)
		{
			float lerpAmount;

			switch (visibility)
			{
				default:
				case VisibilityType.Obscured: lerpAmount = ObscuredAmount; break;
				case VisibilityType.PreviouslySeen: lerpAmount = PreviouslySeenAmount; break;
				case VisibilityType.Visible: lerpAmount = VisibleAmount; break;
			}

			renderer.material.SetColor(colorId, LerpColor);
			renderer.material.SetFloat(lerpAmountId, lerpAmount);
		}
	}
}
