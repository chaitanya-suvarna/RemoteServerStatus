// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
function searchFunction() {
    var input, filter, table, tr, td, i, txtValue, tdata, flag;
    input = document.getElementById("searchStatus");
    filter = input.value.toUpperCase();
    table = document.getElementById("serviceTable");
    tr = table.getElementsByTagName("tr");
    for (i = 0; i < tr.length; i++) {
        tdata = tr[i].getElementsByTagName("td");
        for (j = 0; j < tdata.length; j++) {
            td = tdata[j];
            if (td) {
                txtValue = td.textContent || td.innerText;
                if (txtValue.toUpperCase().indexOf(filter) > -1) {
                    flag = "found";
                }
            }
        }
        if (flag === "found" || tr[i].id == "header") {
            tr[i].style.display = "";
            flag = "";
        }
        else {
            tr[i].style.display = "none";
        }
    }
}