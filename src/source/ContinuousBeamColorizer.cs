using System.Linq;
using UnityEngine;


namespace BMEffects_Remaster
{
    public class ContinuousBeamColorizer : MonoBehaviour
    {
        public Color _color;
        private float _counter;
        private ParticleSystem[] _psList;
        private ParticleSystem _starter;
        private LineRenderer[] _lrList;
        private Vector3 _prevStart;
        private Vector3 _prevEnd;
        private float _width;
        private const float widthMult = 0.1f;
        private bool _started = false;

        private void Awake()
        {
            _color = Color.white;

            _psList = GetComponentsInChildren<ParticleSystem>();
            _starter = _psList.First(x => x.name == "Start");
            _lrList = GetComponentsInChildren<LineRenderer>();
        }

        private void Update()
        {
            _color.a = _counter / 0.25f;
            _counter = Mathf.Max(0f, _counter - Time.deltaTime);

            Color c = _color;
            c.a *= _counter / 0.25f;

            if (_psList.Length > 0)
            {
                foreach (ParticleSystem ps in _psList)
                {
                    var temp = ps.main;
                    temp.startColor = c;

                    ps.transform.localScale = Vector3.one * Mathf.Max(_width, 0.5f) * widthMult;
                }
            }

            if (_lrList.Length > 0)
            {
                foreach (LineRenderer lr in _lrList)
                {
                    lr.startColor = c;
                    lr.endColor = c;
                }
            }

            if (_counter == 0f && _started)
            {
                BMEUtils.PlaySound(AssetRegistryPatch.wave_end, transform.position, 1.5f);
                _started = false;
            }
        }

        public void Fire(Color color, Vector3 start, Vector3 end, Vector3 direction, float width)
        {
            if (_counter == 0f)
            {
                BMEUtils.PlaySound(AssetRegistryPatch.wave_start, start, 1.5f);
                _starter?.Play();
                _started = true;
            }

            _color = color;
            _counter = 0.25f;

            _width = width;

            if (Mathf.Abs((end - _prevEnd).magnitude) > end.magnitude / 2f) _prevEnd = end;
            Vector3 predStart = start * 2f - _prevStart;
            Vector3 predEnd = end * 2f - _prevEnd;

            Vector3 dir = (predEnd - predStart).normalized;

            transform.localPosition = predStart;
            transform.forward = dir;

            if (_lrList.Length > 0)
            {
                Vector3[] positions = new Vector3[2];
                positions[0] = predStart + dir * _width / 10f;
                positions[1] = predEnd + dir * _width / 10f;

                foreach (LineRenderer lr in _lrList)
                {
                    lr.SetPositions(positions);
                    if (lr.name.Contains("Wave")) lr.widthMultiplier = _width * 4f * widthMult;
                    else lr.widthMultiplier = _width * 2f * widthMult;
                }
            }
            _prevStart = start;
            _prevEnd = end;
        }
    }
}