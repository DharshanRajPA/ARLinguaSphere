package com.arlinguasphere.voice;

import android.app.Activity;
import android.speech.SpeechRecognizer;
import android.speech.RecognizerIntent;
import android.content.Intent;
import android.os.Bundle;
import android.speech.RecognitionListener;
import android.speech.tts.TextToSpeech;
import java.util.Locale;

public class SpeechPlugin implements RecognitionListener, TextToSpeech.OnInitListener {
    private final Activity activity;
    private SpeechRecognizer recognizer;
    private Intent recognizerIntent;
    private TextToSpeech tts;

    public SpeechPlugin(Activity activity) {
        this.activity = activity;
        try {
            recognizer = SpeechRecognizer.createSpeechRecognizer(activity);
            recognizer.setRecognitionListener(this);
            tts = new TextToSpeech(activity, this);
        } catch (Throwable t) { /* no-op stub */ }
    }

    public void startListening(String localeTag) {
        if (recognizer == null) return;
        recognizerIntent = new Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH);
        recognizerIntent.putExtra(RecognizerIntent.EXTRA_LANGUAGE_MODEL, RecognizerIntent.LANGUAGE_MODEL_FREE_FORM);
        recognizerIntent.putExtra(RecognizerIntent.EXTRA_PARTIAL_RESULTS, true);
        recognizerIntent.putExtra(RecognizerIntent.EXTRA_LANGUAGE, localeTag);
        recognizer.startListening(recognizerIntent);
    }

    public void stopListening() {
        if (recognizer == null) return;
        recognizer.stopListening();
        recognizer.cancel();
    }

    public void speak(String text, String localeTag, float rate, float pitch, float volume) {
        if (tts == null) return;
        try {
            tts.setLanguage(Locale.forLanguageTag(localeTag));
            tts.setSpeechRate(rate);
            tts.setPitch(pitch);
            // Android TTS has no direct volume per-utterance; handled by audio manager elsewhere.
            tts.speak(text, TextToSpeech.QUEUE_FLUSH, null, "arlinguasphere-utterance");
        } catch (Throwable t) { /* no-op stub */ }
    }

    // RecognitionListener stubs
    @Override public void onReadyForSpeech(Bundle params) {}
    @Override public void onBeginningOfSpeech() {}
    @Override public void onRmsChanged(float rmsdB) {}
    @Override public void onBufferReceived(byte[] buffer) {}
    @Override public void onEndOfSpeech() {}
    @Override public void onError(int error) {}
    @Override public void onResults(Bundle results) {}
    @Override public void onPartialResults(Bundle partialResults) {}
    @Override public void onEvent(int eventType, Bundle params) {}

    // TextToSpeech.OnInitListener
    @Override public void onInit(int status) {}
}


