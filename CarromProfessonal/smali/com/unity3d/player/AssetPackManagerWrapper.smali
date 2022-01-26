.class Lcom/unity3d/player/AssetPackManagerWrapper;
.super Ljava/lang/Object;


# annotations
.annotation system Ldalvik/annotation/MemberClasses;
    value = {
        Lcom/unity3d/player/AssetPackManagerWrapper$c;,
        Lcom/unity3d/player/AssetPackManagerWrapper$b;,
        Lcom/unity3d/player/AssetPackManagerWrapper$a;,
        Lcom/unity3d/player/AssetPackManagerWrapper$e;,
        Lcom/unity3d/player/AssetPackManagerWrapper$d;
    }
.end annotation


# static fields
.field private static a:Lcom/unity3d/player/AssetPackManagerWrapper;


# instance fields
.field private b:Lcom/google/android/play/core/assetpacks/AssetPackManager;

.field private c:Ljava/util/HashSet;

.field private d:Ljava/lang/Object;


# direct methods
.method private constructor <init>(Landroid/content/Context;)V
    .locals 1

    invoke-direct {p0}, Ljava/lang/Object;-><init>()V

    sget-object v0, Lcom/unity3d/player/AssetPackManagerWrapper;->a:Lcom/unity3d/player/AssetPackManagerWrapper;

    if-nez v0, :cond_0

    :try_start_0
    invoke-static {p1}, Lcom/google/android/play/core/assetpacks/AssetPackManagerFactory;->getInstance(Landroid/content/Context;)Lcom/google/android/play/core/assetpacks/AssetPackManager;

    move-result-object p1

    iput-object p1, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->b:Lcom/google/android/play/core/assetpacks/AssetPackManager;
    :try_end_0
    .catch Ljava/lang/NoClassDefFoundError; {:try_start_0 .. :try_end_0} :catch_0

    goto :goto_0

    :catch_0
    const/4 p1, 0x0

    iput-object p1, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->b:Lcom/google/android/play/core/assetpacks/AssetPackManager;

    :goto_0
    new-instance p1, Ljava/util/HashSet;

    invoke-direct {p1}, Ljava/util/HashSet;-><init>()V

    iput-object p1, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->c:Ljava/util/HashSet;

    return-void

    :cond_0
    new-instance p1, Ljava/lang/RuntimeException;

    const-string v0, "AssetPackManagerWrapper should be created only once. Use getInstance() instead."

    invoke-direct {p1, v0}, Ljava/lang/RuntimeException;-><init>(Ljava/lang/String;)V

    throw p1
.end method

.method static synthetic a()Lcom/unity3d/player/AssetPackManagerWrapper;
    .locals 1

    sget-object v0, Lcom/unity3d/player/AssetPackManagerWrapper;->a:Lcom/unity3d/player/AssetPackManagerWrapper;

    return-object v0
.end method

.method static synthetic a(Lcom/unity3d/player/AssetPackManagerWrapper;)Ljava/util/HashSet;
    .locals 0

    iget-object p0, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->c:Ljava/util/HashSet;

    return-object p0
.end method

.method static synthetic a(Lcom/unity3d/player/AssetPackManagerWrapper;Ljava/lang/String;Lcom/unity3d/player/IAssetPackManagerDownloadStatusCallback;Landroid/os/Looper;)V
    .locals 0

    invoke-direct {p0, p1, p2, p3}, Lcom/unity3d/player/AssetPackManagerWrapper;->a(Ljava/lang/String;Lcom/unity3d/player/IAssetPackManagerDownloadStatusCallback;Landroid/os/Looper;)V

    return-void
.end method

.method private a(Ljava/lang/String;Lcom/unity3d/player/IAssetPackManagerDownloadStatusCallback;Landroid/os/Looper;)V
    .locals 2

    sget-object v0, Lcom/unity3d/player/AssetPackManagerWrapper;->a:Lcom/unity3d/player/AssetPackManagerWrapper;

    monitor-enter v0

    :try_start_0
    iget-object v1, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->d:Ljava/lang/Object;

    if-nez v1, :cond_0

    new-instance v1, Lcom/unity3d/player/AssetPackManagerWrapper$b;

    invoke-direct {v1, p0, p2, p3}, Lcom/unity3d/player/AssetPackManagerWrapper$b;-><init>(Lcom/unity3d/player/AssetPackManagerWrapper;Lcom/unity3d/player/IAssetPackManagerDownloadStatusCallback;Landroid/os/Looper;)V

    iget-object p2, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->b:Lcom/google/android/play/core/assetpacks/AssetPackManager;

    invoke-interface {p2, v1}, Lcom/google/android/play/core/assetpacks/AssetPackManager;->registerListener(Lcom/google/android/play/core/assetpacks/AssetPackStateUpdateListener;)V

    iput-object v1, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->d:Ljava/lang/Object;

    goto :goto_0

    :cond_0
    iget-object p3, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->d:Ljava/lang/Object;

    check-cast p3, Lcom/unity3d/player/AssetPackManagerWrapper$b;

    invoke-virtual {p3, p2}, Lcom/unity3d/player/AssetPackManagerWrapper$b;->a(Lcom/unity3d/player/IAssetPackManagerDownloadStatusCallback;)V

    :goto_0
    iget-object p2, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->c:Ljava/util/HashSet;

    invoke-virtual {p2, p1}, Ljava/util/HashSet;->add(Ljava/lang/Object;)Z

    iget-object p2, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->b:Lcom/google/android/play/core/assetpacks/AssetPackManager;

    invoke-static {p1}, Ljava/util/Collections;->singletonList(Ljava/lang/Object;)Ljava/util/List;

    move-result-object p1

    invoke-interface {p2, p1}, Lcom/google/android/play/core/assetpacks/AssetPackManager;->fetch(Ljava/util/List;)Lcom/google/android/play/core/tasks/Task;

    monitor-exit v0

    return-void

    :catchall_0
    move-exception p1

    monitor-exit v0
    :try_end_0
    .catchall {:try_start_0 .. :try_end_0} :catchall_0

    throw p1
.end method

.method static synthetic b(Lcom/unity3d/player/AssetPackManagerWrapper;)Ljava/lang/Object;
    .locals 0

    iget-object p0, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->d:Ljava/lang/Object;

    return-object p0
.end method

.method private b()V
    .locals 2

    invoke-virtual {p0}, Lcom/unity3d/player/AssetPackManagerWrapper;->playCoreApiMissing()Z

    move-result v0

    if-nez v0, :cond_0

    return-void

    :cond_0
    new-instance v0, Ljava/lang/RuntimeException;

    const-string v1, "AssetPackManager API is not available! Make sure your gradle project includes \"com.google.android.play:core\" dependency."

    invoke-direct {v0, v1}, Ljava/lang/RuntimeException;-><init>(Ljava/lang/String;)V

    throw v0
.end method

.method static synthetic c(Lcom/unity3d/player/AssetPackManagerWrapper;)Ljava/lang/Object;
    .locals 1

    const/4 v0, 0x0

    iput-object v0, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->d:Ljava/lang/Object;

    return-object v0
.end method

.method public static declared-synchronized getInstance()Lcom/unity3d/player/AssetPackManagerWrapper;
    .locals 3

    const-class v0, Lcom/unity3d/player/AssetPackManagerWrapper;

    monitor-enter v0

    :goto_0
    :try_start_0
    sget-object v1, Lcom/unity3d/player/AssetPackManagerWrapper;->a:Lcom/unity3d/player/AssetPackManagerWrapper;
    :try_end_0
    .catchall {:try_start_0 .. :try_end_0} :catchall_0

    if-nez v1, :cond_0

    const-wide/16 v1, 0xbb8

    :try_start_1
    invoke-virtual {v0, v1, v2}, Ljava/lang/Object;->wait(J)V
    :try_end_1
    .catch Ljava/lang/InterruptedException; {:try_start_1 .. :try_end_1} :catch_0
    .catchall {:try_start_1 .. :try_end_1} :catchall_0

    goto :goto_0

    :catch_0
    move-exception v1

    const/4 v2, 0x6

    :try_start_2
    invoke-virtual {v1}, Ljava/lang/InterruptedException;->getMessage()Ljava/lang/String;

    move-result-object v1

    invoke-static {v2, v1}, Lcom/unity3d/player/d;->Log(ILjava/lang/String;)V

    goto :goto_0

    :cond_0
    sget-object v1, Lcom/unity3d/player/AssetPackManagerWrapper;->a:Lcom/unity3d/player/AssetPackManagerWrapper;

    if-eqz v1, :cond_1

    sget-object v1, Lcom/unity3d/player/AssetPackManagerWrapper;->a:Lcom/unity3d/player/AssetPackManagerWrapper;
    :try_end_2
    .catchall {:try_start_2 .. :try_end_2} :catchall_0

    monitor-exit v0

    return-object v1

    :cond_1
    :try_start_3
    new-instance v1, Ljava/lang/RuntimeException;

    const-string v2, "AssetPackManagerWrapper is not yet initialised."

    invoke-direct {v1, v2}, Ljava/lang/RuntimeException;-><init>(Ljava/lang/String;)V

    throw v1
    :try_end_3
    .catchall {:try_start_3 .. :try_end_3} :catchall_0

    :catchall_0
    move-exception v1

    monitor-exit v0

    goto :goto_2

    :goto_1
    throw v1

    :goto_2
    goto :goto_1
.end method

.method public static declared-synchronized init(Landroid/content/Context;)Lcom/unity3d/player/AssetPackManagerWrapper;
    .locals 2

    const-class v0, Lcom/unity3d/player/AssetPackManagerWrapper;

    monitor-enter v0

    :try_start_0
    sget-object v1, Lcom/unity3d/player/AssetPackManagerWrapper;->a:Lcom/unity3d/player/AssetPackManagerWrapper;

    if-nez v1, :cond_0

    new-instance v1, Lcom/unity3d/player/AssetPackManagerWrapper;

    invoke-direct {v1, p0}, Lcom/unity3d/player/AssetPackManagerWrapper;-><init>(Landroid/content/Context;)V

    sput-object v1, Lcom/unity3d/player/AssetPackManagerWrapper;->a:Lcom/unity3d/player/AssetPackManagerWrapper;

    invoke-virtual {v0}, Ljava/lang/Object;->notifyAll()V

    sget-object p0, Lcom/unity3d/player/AssetPackManagerWrapper;->a:Lcom/unity3d/player/AssetPackManagerWrapper;
    :try_end_0
    .catchall {:try_start_0 .. :try_end_0} :catchall_0

    monitor-exit v0

    return-object p0

    :cond_0
    :try_start_1
    new-instance p0, Ljava/lang/RuntimeException;

    const-string v1, "AssetPackManagerWrapper.init() should be called only once. Use getInstance() instead."

    invoke-direct {p0, v1}, Ljava/lang/RuntimeException;-><init>(Ljava/lang/String;)V

    throw p0
    :try_end_1
    .catchall {:try_start_1 .. :try_end_1} :catchall_0

    :catchall_0
    move-exception p0

    monitor-exit v0

    throw p0
.end method


# virtual methods
.method public cancelAssetPackDownload(Ljava/lang/String;)V
    .locals 2

    const/4 v0, 0x1

    new-array v0, v0, [Ljava/lang/String;

    const/4 v1, 0x0

    aput-object p1, v0, v1

    invoke-virtual {p0, v0}, Lcom/unity3d/player/AssetPackManagerWrapper;->cancelAssetPackDownloads([Ljava/lang/String;)V

    return-void
.end method

.method public cancelAssetPackDownloads([Ljava/lang/String;)V
    .locals 1

    invoke-direct {p0}, Lcom/unity3d/player/AssetPackManagerWrapper;->b()V

    iget-object v0, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->b:Lcom/google/android/play/core/assetpacks/AssetPackManager;

    invoke-static {p1}, Ljava/util/Arrays;->asList([Ljava/lang/Object;)Ljava/util/List;

    move-result-object p1

    invoke-interface {v0, p1}, Lcom/google/android/play/core/assetpacks/AssetPackManager;->cancel(Ljava/util/List;)Lcom/google/android/play/core/assetpacks/AssetPackStates;

    return-void
.end method

.method public downloadAssetPack(Ljava/lang/String;Lcom/unity3d/player/IAssetPackManagerDownloadStatusCallback;)V
    .locals 2

    const/4 v0, 0x1

    new-array v0, v0, [Ljava/lang/String;

    const/4 v1, 0x0

    aput-object p1, v0, v1

    invoke-virtual {p0, v0, p2}, Lcom/unity3d/player/AssetPackManagerWrapper;->downloadAssetPacks([Ljava/lang/String;Lcom/unity3d/player/IAssetPackManagerDownloadStatusCallback;)V

    return-void
.end method

.method public downloadAssetPacks([Ljava/lang/String;Lcom/unity3d/player/IAssetPackManagerDownloadStatusCallback;)V
    .locals 5

    invoke-direct {p0}, Lcom/unity3d/player/AssetPackManagerWrapper;->b()V

    array-length v0, p1

    const/4 v1, 0x0

    :goto_0
    if-ge v1, v0, :cond_0

    aget-object v2, p1, v1

    iget-object v3, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->b:Lcom/google/android/play/core/assetpacks/AssetPackManager;

    invoke-static {v2}, Ljava/util/Collections;->singletonList(Ljava/lang/Object;)Ljava/util/List;

    move-result-object v4

    invoke-interface {v3, v4}, Lcom/google/android/play/core/assetpacks/AssetPackManager;->getPackStates(Ljava/util/List;)Lcom/google/android/play/core/tasks/Task;

    move-result-object v3

    new-instance v4, Lcom/unity3d/player/AssetPackManagerWrapper$d;

    invoke-direct {v4, p2, v2}, Lcom/unity3d/player/AssetPackManagerWrapper$d;-><init>(Lcom/unity3d/player/IAssetPackManagerDownloadStatusCallback;Ljava/lang/String;)V

    invoke-virtual {v3, v4}, Lcom/google/android/play/core/tasks/Task;->addOnCompleteListener(Lcom/google/android/play/core/tasks/OnCompleteListener;)Lcom/google/android/play/core/tasks/Task;

    add-int/lit8 v1, v1, 0x1

    goto :goto_0

    :cond_0
    return-void
.end method

.method public getAssetPackPath(Ljava/lang/String;)Ljava/lang/String;
    .locals 1

    invoke-direct {p0}, Lcom/unity3d/player/AssetPackManagerWrapper;->b()V

    iget-object v0, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->b:Lcom/google/android/play/core/assetpacks/AssetPackManager;

    invoke-interface {v0, p1}, Lcom/google/android/play/core/assetpacks/AssetPackManager;->getPackLocation(Ljava/lang/String;)Lcom/google/android/play/core/assetpacks/AssetPackLocation;

    move-result-object p1

    if-nez p1, :cond_0

    const-string p1, ""

    return-object p1

    :cond_0
    invoke-virtual {p1}, Lcom/google/android/play/core/assetpacks/AssetPackLocation;->assetsPath()Ljava/lang/String;

    move-result-object p1

    return-object p1
.end method

.method public getAssetPackState(Ljava/lang/String;Lcom/unity3d/player/IAssetPackManagerStatusQueryCallback;)V
    .locals 2

    const/4 v0, 0x1

    new-array v0, v0, [Ljava/lang/String;

    const/4 v1, 0x0

    aput-object p1, v0, v1

    invoke-virtual {p0, v0, p2}, Lcom/unity3d/player/AssetPackManagerWrapper;->getAssetPackStates([Ljava/lang/String;Lcom/unity3d/player/IAssetPackManagerStatusQueryCallback;)V

    return-void
.end method

.method public getAssetPackStates([Ljava/lang/String;Lcom/unity3d/player/IAssetPackManagerStatusQueryCallback;)V
    .locals 2

    invoke-direct {p0}, Lcom/unity3d/player/AssetPackManagerWrapper;->b()V

    iget-object v0, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->b:Lcom/google/android/play/core/assetpacks/AssetPackManager;

    invoke-static {p1}, Ljava/util/Arrays;->asList([Ljava/lang/Object;)Ljava/util/List;

    move-result-object v1

    invoke-interface {v0, v1}, Lcom/google/android/play/core/assetpacks/AssetPackManager;->getPackStates(Ljava/util/List;)Lcom/google/android/play/core/tasks/Task;

    move-result-object v0

    new-instance v1, Lcom/unity3d/player/AssetPackManagerWrapper$e;

    invoke-direct {v1, p2, p1}, Lcom/unity3d/player/AssetPackManagerWrapper$e;-><init>(Lcom/unity3d/player/IAssetPackManagerStatusQueryCallback;[Ljava/lang/String;)V

    invoke-virtual {v0, v1}, Lcom/google/android/play/core/tasks/Task;->addOnCompleteListener(Lcom/google/android/play/core/tasks/OnCompleteListener;)Lcom/google/android/play/core/tasks/Task;

    return-void
.end method

.method public playCoreApiMissing()Z
    .locals 1

    iget-object v0, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->b:Lcom/google/android/play/core/assetpacks/AssetPackManager;

    if-nez v0, :cond_0

    const/4 v0, 0x1

    return v0

    :cond_0
    const/4 v0, 0x0

    return v0
.end method

.method public registerDownloadStatusListener(Lcom/unity3d/player/IAssetPackManagerDownloadStatusCallback;)Ljava/lang/Object;
    .locals 1

    invoke-direct {p0}, Lcom/unity3d/player/AssetPackManagerWrapper;->b()V

    new-instance v0, Lcom/unity3d/player/AssetPackManagerWrapper$b;

    invoke-direct {v0, p0, p1}, Lcom/unity3d/player/AssetPackManagerWrapper$b;-><init>(Lcom/unity3d/player/AssetPackManagerWrapper;Lcom/unity3d/player/IAssetPackManagerDownloadStatusCallback;)V

    iget-object p1, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->b:Lcom/google/android/play/core/assetpacks/AssetPackManager;

    invoke-interface {p1, v0}, Lcom/google/android/play/core/assetpacks/AssetPackManager;->registerListener(Lcom/google/android/play/core/assetpacks/AssetPackStateUpdateListener;)V

    return-object v0
.end method

.method public removeAssetPack(Ljava/lang/String;)V
    .locals 1

    invoke-direct {p0}, Lcom/unity3d/player/AssetPackManagerWrapper;->b()V

    iget-object v0, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->b:Lcom/google/android/play/core/assetpacks/AssetPackManager;

    invoke-interface {v0, p1}, Lcom/google/android/play/core/assetpacks/AssetPackManager;->removePack(Ljava/lang/String;)Lcom/google/android/play/core/tasks/Task;

    return-void
.end method

.method public requestToUseMobileData(Landroid/app/Activity;Lcom/unity3d/player/IAssetPackManagerMobileDataConfirmationCallback;)V
    .locals 1

    invoke-direct {p0}, Lcom/unity3d/player/AssetPackManagerWrapper;->b()V

    iget-object v0, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->b:Lcom/google/android/play/core/assetpacks/AssetPackManager;

    invoke-interface {v0, p1}, Lcom/google/android/play/core/assetpacks/AssetPackManager;->showCellularDataConfirmation(Landroid/app/Activity;)Lcom/google/android/play/core/tasks/Task;

    move-result-object p1

    new-instance v0, Lcom/unity3d/player/AssetPackManagerWrapper$c;

    invoke-direct {v0, p2}, Lcom/unity3d/player/AssetPackManagerWrapper$c;-><init>(Lcom/unity3d/player/IAssetPackManagerMobileDataConfirmationCallback;)V

    invoke-virtual {p1, v0}, Lcom/google/android/play/core/tasks/Task;->addOnSuccessListener(Lcom/google/android/play/core/tasks/OnSuccessListener;)Lcom/google/android/play/core/tasks/Task;

    return-void
.end method

.method public unregisterDownloadStatusListener(Ljava/lang/Object;)V
    .locals 1

    invoke-direct {p0}, Lcom/unity3d/player/AssetPackManagerWrapper;->b()V

    instance-of v0, p1, Lcom/unity3d/player/AssetPackManagerWrapper$b;

    if-nez v0, :cond_0

    return-void

    :cond_0
    iget-object v0, p0, Lcom/unity3d/player/AssetPackManagerWrapper;->b:Lcom/google/android/play/core/assetpacks/AssetPackManager;

    check-cast p1, Lcom/unity3d/player/AssetPackManagerWrapper$b;

    invoke-interface {v0, p1}, Lcom/google/android/play/core/assetpacks/AssetPackManager;->unregisterListener(Lcom/google/android/play/core/assetpacks/AssetPackStateUpdateListener;)V

    return-void
.end method
