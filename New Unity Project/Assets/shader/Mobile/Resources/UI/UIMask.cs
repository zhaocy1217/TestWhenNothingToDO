using UnityEngine.UI;
using UnityEngine;

public class UIMask : RectMask2D
{
	public RectTransform leftBottom;
	public RectTransform rightTop;

	private bool m_bDirty = false;

	protected override void Start()
	{
		base.Start();

		if (leftBottom == null)
		{
			leftBottom = transform.Find("_leftBottom") as RectTransform;
			if (leftBottom == null)
				leftBottom = CreateChild("_leftBottom", Vector2.zero, Vector2.zero);
		}

		if (rightTop == null)
		{
			rightTop = transform.Find("_rightTop") as RectTransform;
			if (rightTop == null)
				rightTop = CreateChild("_rightTop", Vector2.one, Vector2.one);
		}

		RefreshParticleRange();
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		RefreshRange();
	}

	private void LateUpdate()
	{
		if (m_bDirty)
		{
			m_bDirty = false;
			RefreshParticleRange();
		}
	}

	public void RefreshRange()
	{
		m_bDirty = true;
	}

	private void RefreshParticleRange()
	{
		if (leftBottom == null || rightTop == null)
			return;

		float minX, minY, maxX, maxY;
		minX = leftBottom.position.x;
		minY = leftBottom.position.y;
		maxX = rightTop.position.x;
		maxY = rightTop.position.y;

		ParticleSystem[] particlesSystems = gameObject.GetComponentsInChildren<ParticleSystem>(true);
		foreach (ParticleSystem particleSystem in particlesSystems)
		{
			var mat = particleSystem.GetComponent<Renderer>().material;
			if (mat.HasProperty("_MinX"))
			{
				mat.SetFloat("_MinX", minX);
				mat.SetFloat("_MaxX", maxX);
				mat.SetFloat("_MinY", minY);
				mat.SetFloat("_MaxY", maxY);
			}
		}
	}

	private RectTransform CreateChild(string name, Vector2 anchorMin, Vector2 anchorMax)
	{
		GameObject obj = new GameObject(name);
		RectTransform rectTran = obj.AddComponent<RectTransform>();
		rectTran.SetParent(transform);
		rectTran.localScale = Vector3.one;
		rectTran.localRotation = Quaternion.identity;
		rectTran.localPosition = Vector3.zero;
		rectTran.sizeDelta = Vector2.one;
		rectTran.anchorMin = anchorMin;
		rectTran.anchorMax = anchorMax;
		rectTran.anchoredPosition = Vector2.zero;
		return rectTran;
	}
}