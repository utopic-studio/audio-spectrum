using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Obtiene el espectro de audio.
/// 
/// tutorial : https://www.youtube.com/watch?v=wtXirrO-iNA&t=8s
/// detalle  : https://answers.unity.com/questions/157940/getoutputdata-and-getspectrumdata-they-represent-t.html
/// </summary>

public class SoundVisual : MonoBehaviour
{
    private const int SAMPLE_SIZE = 1024;

    public AudioSource source;
    public float visualModifier = 50f;
    public float smoothSpeed = 10f;

    private float rmsValue;
    private float dbValue;
    private float pitchValue;

    private float[] samples;
    private float[] spectrum;
    private float sampleRate;
    private float threshold = 0.02f;     // minimum amplitude to extract pitch

    // Start is called before the first frame update
    void Start()
    {
        if (!source)
            source = GetComponent<AudioSource>();
        samples = new float[SAMPLE_SIZE];
        spectrum = new float[SAMPLE_SIZE];
        sampleRate = AudioSettings.outputSampleRate;


        SpawnLine();

    }

    // Update is called once per frame
    private void Update()
    {
        AnalyzeSound();
        UpdateVisual();
    }

    private void AnalyzeSound()
    {
        source.GetOutputData(samples, 0);

        // get the RMS (Root Mean Square)
        float sum = 0;
        for (int i = 0; i < SAMPLE_SIZE; i++)
        {
            sum += samples[i] * samples[i];
        }
        rmsValue = Mathf.Sqrt(sum / SAMPLE_SIZE);

        // Get DB
        dbValue = 20 * Mathf.Log10(rmsValue / 0.1f);

        // Get Sound Spectrum
        source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        // Get Pitch
        pitchValue = GetPitch();
    }

    private float GetPitch()
    {
        float maxV = 0;
        int maxN = 0;
        for (int i = 0; i < SAMPLE_SIZE; i++)
        { // find max 
            if (!(spectrum[i] > maxV) || !(spectrum[i] > threshold))
                continue;

            maxV = spectrum[i];
            maxN = i;
        }

        float freqN = maxN;
        if (maxN > 0 && maxN < SAMPLE_SIZE - 1)
        { // interpolate index using neighbours
            var dL = spectrum[maxN - 1] / spectrum[maxN];
            var dR = spectrum[maxN + 1] / spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        pitchValue = freqN * (sampleRate / 2) / SAMPLE_SIZE;

        return pitchValue;
    }






    private Transform[] visualList;
    private float[] visualScaleList;
    public int amnVisual = 64;     // cantidad de cubos/columnas creadas
    private void SpawnLine()
    {
        visualList = new Transform[amnVisual];
        visualScaleList = new float[amnVisual];

        for (int i = 0; i < amnVisual; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visualList[i] = go.transform;
            visualList[i].position = Vector3.right * i;

        }
    }
    private void UpdateVisual()
    {
        int visualIndex = 0;
        int spectrumIndex = 0;
        int averageSize = SAMPLE_SIZE / amnVisual;

        while (visualIndex < amnVisual)
        {
            float sum = 0;
            for (int j = 0; j < averageSize; j++)
            {
                sum += spectrum[spectrumIndex];
                spectrumIndex++;
            }

            float scaleY = sum / averageSize * visualModifier;
            visualScaleList[visualIndex] -= Time.deltaTime * smoothSpeed;
            if (visualScaleList[visualIndex] < scaleY)
                visualScaleList[visualIndex] = scaleY;

            visualList[visualIndex].localScale = Vector3.one + Vector3.up * visualScaleList[visualIndex];
            visualIndex++;
        }
    }





}
