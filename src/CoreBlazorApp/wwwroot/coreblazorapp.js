window.CoreBlazorApp = {



    //
    // return base64 iv. base 64 crypted
    //
    EncryptAES: function (paraphrase, salt, plaintext) {
        var key = CryptoJS.PBKDF2(paraphrase, salt, { keySize: 256 / 32, iterations: 1000 });
        var iv = CryptoJS.lib.WordArray.random(128 / 8);
        return iv.toString(CryptoJS.enc.Base64) + "." + CryptoJS.AES.encrypt(plaintext, key, { iv: iv }).toString();
    },
    //
    // return plaintext from base64 iv. base 64 crypted
    //
    DecryptAES: function (paraphrase, salt, encrypted) {
        var key = CryptoJS.PBKDF2(paraphrase, salt, { keySize: 256 / 32, iterations: 1000 });
        var myarray = encrypted.split(".");
        if (myarray.length != 2) return "";
        var iv = CryptoJS.lib.WordArray.create(CryptoJS.enc.Base64.parse(myarray[0])).words;
        var decrypted = CryptoJS.AES.decrypt(myarray[1], key, { iv: iv });
        return decrypted.toString(CryptoJS.enc.Utf8);
    }

}
