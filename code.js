
function CompareStrings(s1, s2) {
    for (var i in s1){
        var foo = s1[i];
        if (s2.indexOf(foo) === -1) {
            return false;
        }
    }
    return true;
};

