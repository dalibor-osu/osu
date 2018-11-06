// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Video;
using osu.Framework.Platform;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Screens;
using osu.Game.Tournament.Components;
using osu.Game.Tournament.Screens.Ladder.Components;
using OpenTK;
using OpenTK.Graphics;

namespace osu.Game.Tournament.Screens.TeamIntro
{
    public class TeamIntroScreen : OsuScreen
    {
        private VideoSprite background;

        [Resolved]
        private Bindable<MatchPairing> currentMatch { get; set; }

        [BackgroundDependencyLoader]
        private void load(Storage storage)
        {
            RelativeSizeAxes = Axes.Both;

            background = new VideoSprite(storage.GetStream(@"BG Team - Both OWC.m4v"))
            {
                RelativeSizeAxes = Axes.Both,
                Loop = true,
            };

            currentMatch.BindValueChanged(matchChanged, true);
        }

        private void matchChanged(MatchPairing pairing)
        {
            if (pairing == null)
                return;

            InternalChildren = new Drawable[]
            {
                background,
                new TeamWithPlayers(pairing.Team1, true)
                {
                    RelativeSizeAxes = Axes.Both,
                    Width = 0.5f,
                    Height = 0.6f,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.CentreRight
                },
                new TeamWithPlayers(pairing.Team2)
                {
                    RelativeSizeAxes = Axes.Both,
                    Width = 0.5f,
                    Height = 0.6f,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.CentreLeft
                },
                new RoundDisplay(pairing.Grouping)
                {
                    RelativeSizeAxes = Axes.Both,
                    Height = 0.25f,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                }
            };
        }

        private class RoundDisplay : CompositeDrawable
        {
            public RoundDisplay(TournamentGrouping group)
            {
                var col = OsuColour.Gray(0.33f);

                InternalChildren = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 10),
                        Children = new Drawable[]
                        {
                            new OsuSpriteText
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Colour = col,
                                Text = "COMING UP NEXT",
                                Font = "Exo2.0-SemiBold",
                                TextSize = 15,
                            },
                            new OsuSpriteText
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Colour = col,
                                Text = group?.Name.Value ?? "Unknown Grouping",
                                Font = "Exo2.0-Light",
                                Spacing = new Vector2(10, 0),
                                TextSize = 50,
                            },
                            new OsuSpriteText
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Colour = col,
                                Text = (group?.StartDate.Value ?? DateTimeOffset.Now).ToString("dd MMMM HH:mm UTC"),
                                TextSize = 20,
                            },
                        }
                    }
                };
            }
        }

        private class TeamWithPlayers : CompositeDrawable
        {
            private readonly Color4 red = new Color4(129, 68, 65, 255);
            private readonly Color4 blue = new Color4(41, 91, 97, 255);

            public TeamWithPlayers(TournamentTeam team, bool left = false)
            {
                FillFlowContainer players;
                var colour = left ? red : blue;
                InternalChildren = new Drawable[]
                {
                    new TeamDisplay(team, left ? "Team Red" : "Team Blue", colour)
                    {
                        Anchor = left ? Anchor.CentreRight : Anchor.CentreLeft,
                        Origin = Anchor.Centre,
                        RelativePositionAxes = Axes.Both,
                        X = (left ? -1 : 1) * 0.36f,
                    },
                    players = new FillFlowContainer
                    {
                        Direction = FillDirection.Vertical,
                        AutoSizeAxes = Axes.Both,
                        Spacing = new Vector2(0, 5),
                        Padding = new MarginPadding(20),
                        Anchor = !left ? Anchor.CentreRight : Anchor.CentreLeft,
                        Origin = !left ? Anchor.CentreRight : Anchor.CentreLeft,
                        RelativePositionAxes = Axes.Both,
                        X = left ? 0.1f : -0.1f,
                    },
                };

                if (team != null)
                {
                    foreach (var p in team.Players)
                        players.Add(new OsuSpriteText
                        {
                            Text = p.Username,
                            TextSize = 24,
                            Colour = colour,
                            Anchor = left ? Anchor.CentreRight : Anchor.CentreLeft,
                            Origin = left ? Anchor.CentreRight : Anchor.CentreLeft,
                        });
                }
            }

            private class TeamDisplay : DrawableTournamentTeam
            {
                public TeamDisplay(TournamentTeam team, string teamName, Color4 colour)
                    : base(team)
                {
                    AutoSizeAxes = Axes.Both;

                    Flag.Anchor = Flag.Origin = Anchor.TopCentre;
                    Flag.RelativeSizeAxes = Axes.None;
                    Flag.Size = new Vector2(300, 200);
                    Flag.Scale = new Vector2(0.4f);
                    Flag.Margin = new MarginPadding { Bottom = 20 };

                    InternalChild = new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 5),
                        Children = new Drawable[]
                        {
                            Flag,
                            new OsuSpriteText
                            {
                                Text = team?.FullName.ToUpper() ?? "???",
                                TextSize = 40,
                                Colour = Color4.Black,
                                Font = "Exo2.0-Light",
                                Origin = Anchor.TopCentre,
                                Anchor = Anchor.TopCentre,
                            },
                            new OsuSpriteText
                            {
                                Text = teamName.ToUpper(),
                                TextSize = 20,
                                Colour = colour,
                                Origin = Anchor.TopCentre,
                                Anchor = Anchor.TopCentre,
                            }
                        }
                    };
                }
            }
        }
    }
}
