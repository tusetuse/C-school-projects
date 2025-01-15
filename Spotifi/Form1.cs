using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NAudio.Wave;

namespace Spotifi
{
    public partial class Form1 : Form
    {
        private IWavePlayer wavePlayer;
        private AudioFileReader audioFile;
        private List<string> playlist;
        private Queue<int> queue;  // Store indices of queued songs
        private int currentTrackIndex;
        private bool isPlaying;
        private string musicFolderPath;

        // Store controls as class members
        private ListView playlistView;
        private ListView queueView;
        private Button playButton;
        private Button prevButton;
        private Button nextButton;
        private Button addToQueueButton;

        public Form1()
        {
            InitializeComponent();
            SetupCustomUI();
            InitializePlayer();

        }

        private void InitializePlayer()
        {
            wavePlayer = new WaveOut();
            playlist = new List<string>();
            queue = new Queue<int>();
            currentTrackIndex = -1;
            isPlaying = false;
            musicFolderPath = @"C:\Users\ivanr\Desktop\kokotiny";
            LoadMusicFromFolder();
        }

        private void SetupCustomUI()
        {
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.ForeColor = Color.White;

            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 2
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 85));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 15));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

            // Create playlist view
            playlistView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White
            };
            playlistView.Columns.Add("Title", -2);
            playlistView.Columns.Add("Duration", 100);
            playlistView.Click += PlaylistView_Click;

            // Create queue view
            queueView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White
            };
            queueView.Columns.Add("Queue", -2);
            queueView.Click += QueueView_Click;

            // Create control panel
            Panel controlPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(24, 24, 24)
            };

            // Create playback controls
            playButton = new Button
            {
                Text = "Play",
                Size = new Size(80, 30),
                Location = new Point(360, 20),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(29, 185, 84),
                ForeColor = Color.White
            };
            playButton.Click += PlayButton_Click;

            prevButton = new Button
            {
                Text = "Previous",
                Size = new Size(80, 30),
                Location = new Point(270, 20),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White
            };
            prevButton.Click += PrevButton_Click;

            nextButton = new Button
            {
                Text = "Next",
                Size = new Size(80, 30),
                Location = new Point(450, 20),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White
            };
            nextButton.Click += NextButton_Click;

            addToQueueButton = new Button
            {
                Text = "Add to Queue",
                Size = new Size(100, 30),
                Location = new Point(540, 20),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White
            };
            addToQueueButton.Click += AddToQueueButton_Click;

            // Add controls to panels
            controlPanel.Controls.AddRange(new Control[] { playButton, prevButton, nextButton, addToQueueButton });

            // Add components to main layout
            mainLayout.Controls.Add(playlistView, 0, 0);
            mainLayout.Controls.Add(queueView, 1, 0);
            mainLayout.Controls.Add(controlPanel, 0, 1);
            mainLayout.SetColumnSpan(controlPanel, 2);

            // Add main layout to form
            this.Controls.Add(mainLayout);
        }

        private void LoadMusicFromFolder()
        {
            if (Directory.Exists(musicFolderPath))
            {
                string[] supportedExtensions = { "*.mp3", "*.wav" };
                List<string> files = new List<string>();

                foreach (string extension in supportedExtensions)
                {
                    files.AddRange(Directory.GetFiles(musicFolderPath, extension));
                }

                foreach (string file in files)
                {
                    AddToPlaylist(file);
                }
                RefreshPlaylistView();
            }
            else
            {
                MessageBox.Show($"Folder not found: {musicFolderPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PlaylistView_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selected = ((ListView)sender).SelectedItems;
            if (selected.Count > 0)
            {
                currentTrackIndex = selected[0].Index;
                PlayCurrentTrack();

                // Update the selected item's appearance
                foreach (ListViewItem item in playlistView.Items)
                {
                    item.BackColor = Color.FromArgb(40, 40, 40);
                    item.ForeColor = Color.White;
                }
                selected[0].BackColor = Color.FromArgb(29, 185, 84);
                selected[0].ForeColor = Color.White;
            }
        }

        private void QueueView_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selected = ((ListView)sender).SelectedItems;
            if (selected.Count > 0)
            {
                currentTrackIndex = (int)selected[0].Tag;
                PlayCurrentTrack();

                // Remove played song from queue
                queue = new Queue<int>(queue.Where(x => x != currentTrackIndex));
                RefreshQueueView();
            }
        }

        private void AddToQueueButton_Click(object sender, EventArgs e)
        {
            if (playlistView.SelectedItems.Count > 0)
            {
                int selectedIndex = playlistView.SelectedItems[0].Index;
                queue.Enqueue(selectedIndex);
                RefreshQueueView();
            }
        }

        private void RefreshQueueView()
        {
            queueView.Items.Clear();
            foreach (int index in queue)
            {
                ListViewItem item = new ListViewItem(Path.GetFileNameWithoutExtension(playlist[index]));
                item.Tag = index;  // Store the playlist index
                queueView.Items.Add(item);
            }
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (isPlaying)
            {
                PausePlayback();
            }
            else
            {
                if (currentTrackIndex == -1)
                {
                    if (queue.Count > 0)
                    {
                        currentTrackIndex = queue.Dequeue();
                        RefreshQueueView();
                    }
                    else if (playlist.Count > 0)
                    {
                        currentTrackIndex = 0;
                    }
                }
                PlayCurrentTrack();
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            PlayNextTrack();
        }

        private void PlayNextTrack()
        {
            if (queue.Count > 0)
            {
                currentTrackIndex = queue.Dequeue();
                RefreshQueueView();
                PlayCurrentTrack();
            }
            else if (currentTrackIndex < playlist.Count - 1)
            {
                currentTrackIndex++;
                PlayCurrentTrack();
            }
        }

        private void PrevButton_Click(object sender, EventArgs e)
        {
            if (currentTrackIndex > 0)
            {
                currentTrackIndex--;
                PlayCurrentTrack();
            }
        }

        private void AddToPlaylist(string filePath)
        {
            playlist.Add(filePath);
        }

        private void RefreshPlaylistView()
        {
            playlistView.Items.Clear();

            foreach (string file in playlist)
            {
                ListViewItem item = new ListViewItem(Path.GetFileNameWithoutExtension(file));
                item.SubItems.Add(GetAudioDuration(file));
                playlistView.Items.Add(item);
            }
        }

        private string GetAudioDuration(string filePath)
        {
            using (var audio = new AudioFileReader(filePath))
            {
                return audio.TotalTime.ToString(@"mm\:ss");
            }
        }

        private void PlayCurrentTrack()
        {
            if (currentTrackIndex >= 0 && currentTrackIndex < playlist.Count)
            {
                if (wavePlayer != null)
                {
                    wavePlayer.Stop();
                    wavePlayer.Dispose();
                    if (audioFile != null)
                    {
                        audioFile.Dispose();
                    }
                }

                wavePlayer = new WaveOut();
                audioFile = new AudioFileReader(playlist[currentTrackIndex]);
                wavePlayer.Init(audioFile);
                wavePlayer.Play();
                isPlaying = true;
                UpdatePlayButtonText();

                // Highlight current track in playlist
                foreach (ListViewItem item in playlistView.Items)
                {
                    item.BackColor = item.Index == currentTrackIndex ?
                        Color.FromArgb(29, 185, 84) : Color.FromArgb(40, 40, 40);
                    item.ForeColor = Color.White;
                }

                // Set up PlaybackStopped event handler for auto-next
                wavePlayer.PlaybackStopped += (s, e) =>
                {
                    if (isPlaying) // Only auto-play next if we weren't manually stopped
                    {
                        this.BeginInvoke(new Action(() => PlayNextTrack()));
                    }
                };
            }
        }

        private void PausePlayback()
        {
            if (wavePlayer != null && isPlaying)
            {
                wavePlayer.Pause();
                isPlaying = false;
                UpdatePlayButtonText();
            }
        }

        private void UpdatePlayButtonText()
        {
            if (playButton != null)
            {
                playButton.Text = isPlaying ? "Pause" : "Play";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (wavePlayer != null)
            {
                wavePlayer.Stop();
                wavePlayer.Dispose();
            }
            if (audioFile != null)
            {
                audioFile.Dispose();
            }
        }
    }
}