using UnityEngine;


namespace BMEffects_Remaster
{
    public class PulseBeamColorizer : MonoBehaviour
    {
        private AnimationCurve _sizeCurve;
        private Gradient _gradient;
        private ParticleSystem[] _psList;
        private LineRenderer _lr;
        private float _counter;
        private float _width;
        private Vector3 _start;
        private Vector3 _end;
        private Vector3[] _positions;

        private const float widthMult = 0.6f;
        private const float timer = 0.7f;

        private void Awake()
        {
            _positions = new Vector3[2];
            _sizeCurve = new AnimationCurve(
                new Keyframe(0f, 1f) { inTangent = 0f, outTangent = 0f, inWeight = 0.333333343f, outWeight = 0.0587084144f, weightedMode = WeightedMode.None },
                new Keyframe(0.08047433f, 0.976530135f) { inTangent = -1.41855145f, outTangent = -1.41855145f, inWeight = 0.333333343f, outWeight = 0.333333343f, weightedMode = WeightedMode.None },
                new Keyframe(0.0823226944f, 0.543670654f) { inTangent = -110.336021f, outTangent = -110.336021f, inWeight = 0f, outWeight = 0.231260046f, weightedMode = WeightedMode.None },
                new Keyframe(0.0837890953f, 0.3f) { inTangent = -0.32798937f, outTangent = -0.32798937f, inWeight = 0.09085188f, outWeight = 0.103767239f, weightedMode = WeightedMode.None },
                new Keyframe(1f, 0f) { inTangent = -0.327435553f, outTangent = -0.327435553f, inWeight = 0.333333343f, outWeight = 0f, weightedMode = WeightedMode.None }
            );
            _sizeCurve.preWrapMode = WrapMode.ClampForever;
            _sizeCurve.postWrapMode = WrapMode.ClampForever;

            _gradient = new Gradient();
            _psList = GetComponentsInChildren<ParticleSystem>();
            _lr = GetComponentInChildren<LineRenderer>();
        }

        public void Fire(Color color, Vector3 start, Vector3 end, float width)
        {
            _counter = timer;
            _width = width;
            _start = start;
            _end = end;

            if (_lr != null)
            {
                GradientAlphaKey gak0 = new GradientAlphaKey(color.a, 0.18f);
                GradientAlphaKey gak1 = new GradientAlphaKey(color.a / 2, 0.2f);
                GradientAlphaKey gak2 = new GradientAlphaKey(0f, 1f);
                GradientColorKey gck = new GradientColorKey(color, 0);
                GradientAlphaKey[] gakList = { gak0, gak1, gak2 };
                GradientColorKey[] gckList = { gck };
                _gradient.SetKeys(gckList, gakList);
            }

            if (_psList.Length > 0)
            {
                foreach (ParticleSystem ps in _psList)
                {
                    if (ps.name.Contains("Flash"))
                    {
                        var temp = ps.main;
                        temp.startColor = color;

                        ps.transform.localScale = Vector3.one * width * widthMult;
                    }
                }
            }
        }

        private void Update()
        {
            if (_counter <= 0f)
                return;

            _counter -= Time.deltaTime;

            float t = Mathf.Clamp01(1f - _counter / timer);
            Color c = _gradient.Evaluate(t);

            _lr.startColor = c;
            _lr.endColor = c;

            float width = _width * widthMult * 2f * _sizeCurve.Evaluate(t);

            Vector3 offset = (_end - _start).normalized * width * 0.5f;
            _positions[0] = _start + offset;
            _positions[1] = _end + offset;
            _lr.SetPositions(_positions);

            _lr.widthMultiplier = width;
        }
    }
}