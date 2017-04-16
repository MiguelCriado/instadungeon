using DG.Tweening;
using InstaDungeon.Configuration;
using RSG;
using UnityEngine;

namespace InstaDungeon.Components
{
	public class ItemInteraction : MonoBehaviour
	{
		private static readonly string ReferencesName = "References";
		private static readonly string TargetName = "Item Interactions";
		private static readonly Vector3 TargetLocalPosition = Vector3.up * 1.25f;

		private static readonly string ForegroundLayerName = "Top";

		[Header("References")]
		[SerializeField] private Transform interactionTarget;

		private void Reset()
		{
			Transform referencesTransform = transform.Find(ReferencesName);

			if (referencesTransform == null)
			{
				GameObject go = new GameObject(ReferencesName);
				referencesTransform = go.transform;
				referencesTransform.SetParent(transform);
				referencesTransform.localPosition = Vector3.zero;
			}

			interactionTarget = referencesTransform.Find(TargetName);

			if (interactionTarget == null)
			{
				GameObject go = new GameObject(TargetName);
				interactionTarget = go.transform;
				interactionTarget.SetParent(referencesTransform);
				interactionTarget.localPosition = TargetLocalPosition;
			}
		}

		private void Start()
		{
			DOTween.Init();
		}

		public IPromise AddItem(ItemInfo item)
		{
			Promise result = new Promise();

			SpriteRenderer itemAvatar = CreateItemAvatar(item);
			itemAvatar.transform.SetParent(interactionTarget);
			itemAvatar.transform.localPosition = Vector3.zero;
			itemAvatar.color = new Color(itemAvatar.color.r, itemAvatar.color.g, itemAvatar.color.b, 0f);

			DOTween.Sequence()
				.Append
				(
					itemAvatar.DOFade(1f, 0.3f)
				)
				.Join
				(
					itemAvatar.transform.DOLocalJump
					(
						Vector3.zero,
						0.5f,
						1,
						0.5f
					)
				)
				.AppendInterval(0.3f)
				.Append
				(
					itemAvatar.DOFade(0f, 0.5f).SetEase(Ease.InExpo)
				)
				.Join
				(
					itemAvatar.transform.DOLocalMoveY
					(
						-TargetLocalPosition.y + 0.5f,
						0.5f
					)
					.SetEase(Ease.InExpo)
				)
				.OnComplete
				(
					() => result.Resolve()
				);

			return result;
		}

		private SpriteRenderer CreateItemAvatar(ItemInfo item)
		{
			GameObject result = new GameObject("Item");
			SpriteRenderer renderer = result.AddComponent<SpriteRenderer>();
			renderer.sprite = item.Avatar;
			renderer.sortingLayerName = ForegroundLayerName;

			return renderer;
		}
	}
}
