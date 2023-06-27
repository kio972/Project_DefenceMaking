using UnityEngine;

public class LineRendererSinWave : MonoBehaviour
{
	[SerializeField]
	private	float			start = 0;			// 선의 시작 x 위치
	[SerializeField]
	private	float			end = 5;			// 선의 끝 x 위치
	[SerializeField][Range(5, 50)]
	private	int				points = 5;			// 점의 개수 (많을수록 부드러운 곡선 표현)
	[SerializeField][Min(0.1f)]
	private	float			amplitude = 1;		// 진폭 (Sin 그래프의 y축 높이)
	[SerializeField][Min(0.5f)]
	private	float			frequency = 1;		// 진동수

	private	LineRenderer	lineRenderer;

	private void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
	}

	private void Update()
	{
		Play();
	}

	private void Play()
	{
		lineRenderer.positionCount = points;

		for ( int i = 0; i < points; ++ i )
		{
			// i를 0.0-1.0 사이의 값으로 정규화
			float t = (float)i / (points - 1);
			// start부터 end 위치까지 points 개수의 점을 일정하게 배치
			float x = Mathf.Lerp(start, end, t);
			// 2*Mathf.PI = 360이고, t는 0.0~1.0 사이의 값이기 때문에 이 값을 곱하면 1 진동의 사인 그래프가 완성되고,
			// frequency를 곱하기 때문에 frequency 값에 따라 진동수가 결정된다.
			float y = amplitude * Mathf.Sin(2 * Mathf.PI * t * frequency);
			
			lineRenderer.SetPosition(i, new Vector3(x, y, 0));
		}
	}
}

