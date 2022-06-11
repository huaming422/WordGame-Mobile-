using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SpriteAnimatonCtrl : MonoBehaviour
{
	public int widthCount;

	public int heightCount;

	public int allCount;

	public float speed = 1f;

	public Material animationMaterial;

	private Image image;

	private void Start()
	{
		image = GetComponent<Image>();
		image.material = animationMaterial;
		animationMaterial.SetFloat("_Speed", speed);
		animationMaterial.SetFloat("_HriCount", widthCount);
		animationMaterial.SetFloat("_VerCount", heightCount);
		animationMaterial.SetFloat("_AllCount", allCount);
	}
}
