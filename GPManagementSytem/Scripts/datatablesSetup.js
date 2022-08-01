$(function () {

    $('#simpleManage').DataTable({
        "order": [],
        "pageLength": 50,
        columnDefs: [
            { targets: [0], orderable: false },
            { targets: [1], orderable: false },
            { targets: [2], orderable: false },
            { targets: [4], orderable: false },
            { targets: [5], orderable: false },
            { targets: [-1], orderable: false } // -1 is the last column
        ]
    });

    $('#ataglance').DataTable({
        "order": [],
        "pageLength": 25
    });
});
