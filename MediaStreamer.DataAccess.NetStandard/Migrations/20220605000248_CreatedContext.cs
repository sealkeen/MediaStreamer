using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MediaStreamer.DataAccess.NetStandard.Migrations
{
    public partial class CreatedContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artist",
                columns: table => new
                {
                    ArtistID = table.Column<long>(type: "bigint", nullable: false),
                    ArtistName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artist", x => x.ArtistID);
                });

            migrationBuilder.CreateTable(
                name: "Genre",
                columns: table => new
                {
                    GenreID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GenreName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genre", x => x.GenreID);
                });

            migrationBuilder.CreateTable(
                name: "Picture",
                columns: table => new
                {
                    PictureID = table.Column<long>(type: "bigint", nullable: false),
                    XResolution = table.Column<long>(type: "bigint", nullable: true),
                    YResolution = table.Column<long>(type: "bigint", nullable: true),
                    SizeKb = table.Column<long>(type: "bigint", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Picture", x => x.PictureID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DateOfSignUp = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    VKLink = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FaceBookLink = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Video",
                columns: table => new
                {
                    VideoID = table.Column<long>(type: "bigint", nullable: false),
                    XResolution = table.Column<long>(type: "bigint", nullable: true),
                    YResolution = table.Column<long>(type: "bigint", nullable: true),
                    FPS = table.Column<double>(type: "float", nullable: true),
                    VariableFPS = table.Column<bool>(type: "BIT", nullable: true),
                    SizeKb = table.Column<long>(type: "bigint", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Video", x => x.VideoID);
                });

            migrationBuilder.CreateTable(
                name: "Album",
                columns: table => new
                {
                    AlbumID = table.Column<long>(type: "bigint", nullable: false),
                    AlbumName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ArtistID = table.Column<long>(type: "bigint", nullable: true),
                    GenreID = table.Column<long>(type: "bigint", nullable: false),
                    Year = table.Column<long>(type: "bigint", nullable: true),
                    Label = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Album", x => x.AlbumID);
                    table.ForeignKey(
                        name: "FK_Album_Artist_ArtistID",
                        column: x => x.ArtistID,
                        principalTable: "Artist",
                        principalColumn: "ArtistID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Album_Genre_GenreID",
                        column: x => x.GenreID,
                        principalTable: "Genre",
                        principalColumn: "GenreID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArtistGenre",
                columns: table => new
                {
                    ArtistID = table.Column<long>(type: "bigint", nullable: false),
                    GenreID = table.Column<long>(type: "bigint", nullable: false),
                    GenreName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DateOfApplication = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistGenre", x => new { x.ArtistID, x.GenreID });
                    table.ForeignKey(
                        name: "FK_ArtistGenre_Artist_ArtistID",
                        column: x => x.ArtistID,
                        principalTable: "Artist",
                        principalColumn: "ArtistID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArtistGenre_Genre_GenreID",
                        column: x => x.GenreID,
                        principalTable: "Genre",
                        principalColumn: "GenreID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Moderator",
                columns: table => new
                {
                    ModeratorID = table.Column<long>(type: "bigint", nullable: false),
                    UserID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moderator", x => x.ModeratorID);
                    table.ForeignKey(
                        name: "FK_Moderator_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AlbumGenre",
                columns: table => new
                {
                    GenreID = table.Column<long>(type: "bigint", nullable: false),
                    AlbumID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumGenre", x => new { x.AlbumID, x.GenreID });
                    table.ForeignKey(
                        name: "FK_AlbumGenre_Album_AlbumID",
                        column: x => x.AlbumID,
                        principalTable: "Album",
                        principalColumn: "AlbumID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AlbumGenre_Genre_GenreID",
                        column: x => x.GenreID,
                        principalTable: "Genre",
                        principalColumn: "GenreID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Composition",
                columns: table => new
                {
                    CompositionID = table.Column<long>(type: "bigint", nullable: false),
                    CompositionName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ArtistID = table.Column<long>(type: "bigint", nullable: true),
                    AlbumID = table.Column<long>(type: "bigint", nullable: true),
                    Duration = table.Column<long>(type: "bigint", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(324)", maxLength: 324, nullable: true),
                    Lyrics = table.Column<string>(type: "nvarchar(3600)", maxLength: 3600, nullable: true),
                    About = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Composition", x => x.CompositionID);
                    table.ForeignKey(
                        name: "FK_Composition_Album_AlbumID",
                        column: x => x.AlbumID,
                        principalTable: "Album",
                        principalColumn: "AlbumID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Composition_Artist_ArtistID",
                        column: x => x.ArtistID,
                        principalTable: "Artist",
                        principalColumn: "ArtistID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Administrator",
                columns: table => new
                {
                    AdministratorID = table.Column<long>(type: "bigint", nullable: false),
                    ModeratorID = table.Column<long>(type: "bigint", nullable: true),
                    UserID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrator", x => x.AdministratorID);
                    table.ForeignKey(
                        name: "FK_Administrator_Moderator_ModeratorID",
                        column: x => x.ModeratorID,
                        principalTable: "Moderator",
                        principalColumn: "ModeratorID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Administrator_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompositionVideo",
                columns: table => new
                {
                    VideoID = table.Column<long>(type: "bigint", nullable: false),
                    CompositionID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompositionVideo", x => new { x.VideoID, x.CompositionID });
                    table.ForeignKey(
                        name: "FK_CompositionVideo_Composition_CompositionID",
                        column: x => x.CompositionID,
                        principalTable: "Composition",
                        principalColumn: "CompositionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompositionVideo_Video_VideoID",
                        column: x => x.VideoID,
                        principalTable: "Video",
                        principalColumn: "VideoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListenedComposition",
                columns: table => new
                {
                    ListenDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    CompositionID = table.Column<long>(type: "bigint", nullable: false),
                    ListenedCompositionID = table.Column<long>(type: "bigint", nullable: false),
                    CountOfPlays = table.Column<long>(type: "bigint", nullable: true),
                    StoppedAt = table.Column<double>(type: "float", nullable: false),
                    ArtistID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListenedComposition", x => new { x.ListenDate, x.UserID, x.CompositionID });
                    table.ForeignKey(
                        name: "FK_ListenedComposition_Artist_ArtistID",
                        column: x => x.ArtistID,
                        principalTable: "Artist",
                        principalColumn: "ArtistID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListenedComposition_Composition_CompositionID",
                        column: x => x.CompositionID,
                        principalTable: "Composition",
                        principalColumn: "CompositionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListenedComposition_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administrator_ModeratorID",
                table: "Administrator",
                column: "ModeratorID");

            migrationBuilder.CreateIndex(
                name: "IX_Administrator_UserID",
                table: "Administrator",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Album_ArtistID",
                table: "Album",
                column: "ArtistID");

            migrationBuilder.CreateIndex(
                name: "IX_Album_GenreID",
                table: "Album",
                column: "GenreID");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumGenre_GenreID",
                table: "AlbumGenre",
                column: "GenreID");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistGenre_GenreID",
                table: "ArtistGenre",
                column: "GenreID");

            migrationBuilder.CreateIndex(
                name: "IX_Composition_AlbumID",
                table: "Composition",
                column: "AlbumID");

            migrationBuilder.CreateIndex(
                name: "IX_Composition_ArtistID",
                table: "Composition",
                column: "ArtistID");

            migrationBuilder.CreateIndex(
                name: "IX_CompositionVideo_CompositionID",
                table: "CompositionVideo",
                column: "CompositionID");

            migrationBuilder.CreateIndex(
                name: "IX_ListenedComposition_ArtistID",
                table: "ListenedComposition",
                column: "ArtistID");

            migrationBuilder.CreateIndex(
                name: "IX_ListenedComposition_CompositionID",
                table: "ListenedComposition",
                column: "CompositionID");

            migrationBuilder.CreateIndex(
                name: "IX_ListenedComposition_UserID",
                table: "ListenedComposition",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Moderator_UserID",
                table: "Moderator",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_UserName",
                table: "User",
                column: "UserName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrator");

            migrationBuilder.DropTable(
                name: "AlbumGenre");

            migrationBuilder.DropTable(
                name: "ArtistGenre");

            migrationBuilder.DropTable(
                name: "CompositionVideo");

            migrationBuilder.DropTable(
                name: "ListenedComposition");

            migrationBuilder.DropTable(
                name: "Picture");

            migrationBuilder.DropTable(
                name: "Moderator");

            migrationBuilder.DropTable(
                name: "Video");

            migrationBuilder.DropTable(
                name: "Composition");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Album");

            migrationBuilder.DropTable(
                name: "Artist");

            migrationBuilder.DropTable(
                name: "Genre");
        }
    }
}
