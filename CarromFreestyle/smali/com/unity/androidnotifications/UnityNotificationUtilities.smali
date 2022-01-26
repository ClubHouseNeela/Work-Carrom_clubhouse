.class public Lcom/unity/androidnotifications/UnityNotificationUtilities;
.super Ljava/lang/Object;
.source "UnityNotificationUtilities.java"


# direct methods
.method public constructor <init>()V
    .locals 0

    .line 13
    invoke-direct {p0}, Ljava/lang/Object;-><init>()V

    return-void
.end method

.method protected static deserializeNotificationIntent(Landroid/content/Context;Ljava/lang/String;)Landroid/content/Intent;
    .locals 4

    const/4 v0, 0x0

    .line 44
    invoke-static {p1, v0}, Landroid/util/Base64;->decode(Ljava/lang/String;I)[B

    move-result-object p1

    .line 46
    new-instance v1, Landroid/os/Bundle;

    invoke-direct {v1}, Landroid/os/Bundle;-><init>()V

    .line 47
    invoke-static {}, Landroid/os/Parcel;->obtain()Landroid/os/Parcel;

    move-result-object v2

    .line 48
    array-length v3, p1

    invoke-virtual {v2, p1, v0, v3}, Landroid/os/Parcel;->unmarshall([BII)V

    .line 49
    invoke-virtual {v2, v0}, Landroid/os/Parcel;->setDataPosition(I)V

    .line 50
    invoke-virtual {v1, v2}, Landroid/os/Bundle;->readFromParcel(Landroid/os/Parcel;)V

    .line 52
    new-instance p1, Landroid/content/Intent;

    const-class v0, Lcom/unity/androidnotifications/UnityNotificationManager;

    invoke-direct {p1, p0, v0}, Landroid/content/Intent;-><init>(Landroid/content/Context;Ljava/lang/Class;)V

    .line 53
    invoke-virtual {p1, v1}, Landroid/content/Intent;->putExtras(Landroid/os/Bundle;)Landroid/content/Intent;

    return-object p1
.end method

.method protected static findResourceIdInContextByName(Landroid/content/Context;Ljava/lang/String;)I
    .locals 4

    const/4 v0, 0x0

    if-nez p1, :cond_0

    return v0

    .line 19
    :cond_0
    :try_start_0
    invoke-virtual {p0}, Landroid/content/Context;->getResources()Landroid/content/res/Resources;

    move-result-object v1

    if-eqz v1, :cond_2

    const-string v2, "mipmap"

    .line 21
    invoke-virtual {p0}, Landroid/content/Context;->getPackageName()Ljava/lang/String;

    move-result-object v3

    invoke-virtual {v1, p1, v2, v3}, Landroid/content/res/Resources;->getIdentifier(Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;)I

    move-result v2

    if-nez v2, :cond_1

    const-string v2, "drawable"

    .line 23
    invoke-virtual {p0}, Landroid/content/Context;->getPackageName()Ljava/lang/String;

    move-result-object p0

    invoke-virtual {v1, p1, v2, p0}, Landroid/content/res/Resources;->getIdentifier(Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;)I

    move-result p0
    :try_end_0
    .catch Landroid/content/res/Resources$NotFoundException; {:try_start_0 .. :try_end_0} :catch_0

    return p0

    :cond_1
    return v2

    :catch_0
    :cond_2
    return v0
.end method

.method protected static getOpenAppActivity(Landroid/content/Context;Z)Ljava/lang/Class;
    .locals 5
    .annotation system Ldalvik/annotation/Signature;
        value = {
            "(",
            "Landroid/content/Context;",
            "Z)",
            "Ljava/lang/Class<",
            "*>;"
        }
    .end annotation

    const/4 v0, 0x0

    .line 61
    :try_start_0
    invoke-virtual {p0}, Landroid/content/Context;->getPackageManager()Landroid/content/pm/PackageManager;

    move-result-object v1

    invoke-virtual {p0}, Landroid/content/Context;->getPackageName()Ljava/lang/String;

    move-result-object v2

    const/16 v3, 0x80

    invoke-virtual {v1, v2, v3}, Landroid/content/pm/PackageManager;->getApplicationInfo(Ljava/lang/String;I)Landroid/content/pm/ApplicationInfo;

    move-result-object v1
    :try_end_0
    .catch Landroid/content/pm/PackageManager$NameNotFoundException; {:try_start_0 .. :try_end_0} :catch_0

    goto :goto_0

    :catch_0
    move-exception v1

    .line 63
    invoke-virtual {v1}, Landroid/content/pm/PackageManager$NameNotFoundException;->printStackTrace()V

    move-object v1, v0

    .line 65
    :goto_0
    iget-object v1, v1, Landroid/content/pm/ApplicationInfo;->metaData:Landroid/os/Bundle;

    const-string v2, "custom_notification_android_activity"

    .line 70
    invoke-virtual {v1, v2}, Landroid/os/Bundle;->containsKey(Ljava/lang/String;)Z

    move-result v3

    if-eqz v3, :cond_0

    .line 71
    invoke-virtual {v1, v2}, Landroid/os/Bundle;->getString(Ljava/lang/String;)Ljava/lang/String;

    move-result-object v1

    .line 74
    :try_start_1
    invoke-static {v1}, Ljava/lang/Class;->forName(Ljava/lang/String;)Ljava/lang/Class;

    move-result-object v0
    :try_end_1
    .catch Ljava/lang/ClassNotFoundException; {:try_start_1 .. :try_end_1} :catch_1

    goto :goto_1

    :catch_1
    nop

    :cond_0
    :goto_1
    if-nez v0, :cond_1

    if-eqz p1, :cond_1

    const-string p1, "UnityNotifications"

    const-string v1, "No custom_notification_android_activity found, attempting to find app activity class"

    .line 81
    invoke-static {p1, v1}, Landroid/util/Log;->w(Ljava/lang/String;Ljava/lang/String;)I

    const-string v1, "com.unity3d.player.UnityPlayerActivity"

    .line 85
    :try_start_2
    invoke-static {v1}, Ljava/lang/Class;->forName(Ljava/lang/String;)Ljava/lang/Class;

    move-result-object p0
    :try_end_2
    .catch Ljava/lang/ClassNotFoundException; {:try_start_2 .. :try_end_2} :catch_2

    return-object p0

    :catch_2
    const/4 v2, 0x1

    new-array v3, v2, [Ljava/lang/Object;

    const/4 v4, 0x0

    aput-object v1, v3, v4

    const-string v1, "Attempting to find : %s, failed!"

    .line 87
    invoke-static {v1, v3}, Ljava/lang/String;->format(Ljava/lang/String;[Ljava/lang/Object;)Ljava/lang/String;

    move-result-object v1

    invoke-static {p1, v1}, Landroid/util/Log;->w(Ljava/lang/String;Ljava/lang/String;)I

    new-array v1, v2, [Ljava/lang/Object;

    .line 88
    invoke-virtual {p0}, Landroid/content/Context;->getPackageName()Ljava/lang/String;

    move-result-object p0

    aput-object p0, v1, v4

    const-string p0, "%s.UnityPlayerActivity"

    invoke-static {p0, v1}, Ljava/lang/String;->format(Ljava/lang/String;[Ljava/lang/Object;)Ljava/lang/String;

    move-result-object p0

    .line 90
    :try_start_3
    invoke-static {p0}, Ljava/lang/Class;->forName(Ljava/lang/String;)Ljava/lang/Class;

    move-result-object p0
    :try_end_3
    .catch Ljava/lang/ClassNotFoundException; {:try_start_3 .. :try_end_3} :catch_3

    return-object p0

    :catch_3
    new-array v1, v2, [Ljava/lang/Object;

    aput-object p0, v1, v4

    const-string p0, "Attempting to find class based on package name: %s, failed!"

    .line 92
    invoke-static {p0, v1}, Ljava/lang/String;->format(Ljava/lang/String;[Ljava/lang/Object;)Ljava/lang/String;

    move-result-object p0

    invoke-static {p1, p0}, Landroid/util/Log;->w(Ljava/lang/String;Ljava/lang/String;)I

    :cond_1
    return-object v0
.end method

.method protected static serializeNotificationIntent(Landroid/content/Intent;)Ljava/lang/String;
    .locals 2

    .line 34
    invoke-virtual {p0}, Landroid/content/Intent;->getExtras()Landroid/os/Bundle;

    move-result-object p0

    .line 36
    invoke-static {}, Landroid/os/Parcel;->obtain()Landroid/os/Parcel;

    move-result-object v0

    const/4 v1, 0x0

    .line 37
    invoke-virtual {p0, v0, v1}, Landroid/os/Bundle;->writeToParcel(Landroid/os/Parcel;I)V

    .line 38
    invoke-virtual {v0}, Landroid/os/Parcel;->marshall()[B

    move-result-object p0

    .line 40
    array-length v0, p0

    invoke-static {p0, v1, v0, v1}, Landroid/util/Base64;->encodeToString([BIII)Ljava/lang/String;

    move-result-object p0

    return-object p0
.end method
