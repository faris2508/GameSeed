using UnityEngine;
using UnityEngine.Playables; // Wajib untuk akses Timeline

public class TimelineStarter : MonoBehaviour
{
    public PlayableDirector timeline; // Drag & drop timeline cutscene ke sini
    public GameObject player; // Drag & drop player

    void Start()
    {
        // Pastikan player tidak bisa bergerak saat cutscene mulai
        if (player != null)
        {
            player.GetComponent<PlayerMovement>().enabled = false;
        }

        // Putar timeline saat game mulai
        if (timeline != null)
        {
            timeline.Play();
            timeline.stopped += OnTimelineFinished;
        }
    }

    void OnTimelineFinished(PlayableDirector director)
    {
        // Aktifkan player setelah timeline selesai
        if (player != null)
        {
            player.GetComponent<PlayerMovement>().enabled = true;
        }

        // Lepas event supaya tidak dipanggil lagi
        timeline.stopped -= OnTimelineFinished;
    }
}
