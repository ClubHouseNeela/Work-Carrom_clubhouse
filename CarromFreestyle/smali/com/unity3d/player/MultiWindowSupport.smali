.class public Lcom/unity3d/player/MultiWindowSupport;
.super Ljava/lang/Object;


# static fields
.field private static final RESIZABLE_WINDOW:Ljava/lang/String; = "unity.allow-resizable-window"


# direct methods
.method public constructor <init>()V
    .locals 0

    invoke-direct {p0}, Ljava/lang/Object;-><init>()V

    return-void
.end method

.method public static getAllowResizableWindow(Landroid/app/Activity;)Z
    .locals 2

    const/4 v0, 0x0

    :try_start_0
    invoke-static {p0}, Lcom/unity3d/player/MultiWindowSupport;->isInMultiWindowMode(Landroid/app/Activity;)Z

    move-result v1

    if-eqz v1, :cond_0

    invoke-virtual {p0}, Landroid/app/Activity;->getApplicationInfo()Landroid/content/pm/ApplicationInfo;

    move-result-object p0

    iget-object p0, p0, Landroid/content/pm/ApplicationInfo;->metaData:Landroid/os/Bundle;

    const-string v1, "unity.allow-resizable-window"

    invoke-virtual {p0, v1}, Landroid/os/Bundle;->getBoolean(Ljava/lang/String;)Z

    move-result p0
    :try_end_0
    .catch Ljava/lang/Exception; {:try_start_0 .. :try_end_0} :catch_0

    if-eqz p0, :cond_0

    const/4 p0, 0x1

    return p0

    :catch_0
    :cond_0
    return v0
.end method

.method static isInMultiWindowMode(Landroid/app/Activity;)Z
    .locals 1

    sget-boolean v0, Lcom/unity3d/player/PlatformSupport;->NOUGAT_SUPPORT:Z

    if-eqz v0, :cond_0

    invoke-virtual {p0}, Landroid/app/Activity;->isInMultiWindowMode()Z

    move-result p0

    return p0

    :cond_0
    const/4 p0, 0x0

    return p0
.end method
