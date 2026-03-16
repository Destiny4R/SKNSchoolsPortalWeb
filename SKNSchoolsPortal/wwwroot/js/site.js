// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    if ($("#fee_payment").length) {
        $("#fee_payment").DataTable({
            "processing": true,
            "serverSide": true,
            "filter": true,
            responsive: true,
            destroy: true,
            "ajax": {
                "url": "/api/FeesPayemtData",
                "type": "POST",
                "datatype": "json"
            },
            "columnDefs": [{ "visible": true, "searchable": true, }],
            "columns": [
                {
                    "data": "id", render: function (data, type, row, meta) { return meta.row + meta.settings._iDisplayStart + 1; }
                },
                {
                    "data": null, "render": function (data, type, full) {
                        var name = data.termregistrations.studentsData.surName + ' ' + (data?.termregistrations.studentsData.otherName || '') + ' ' + data.termregistrations.studentsData.firstName;
                        return name;
                    }
                },
                { "data": "termlyFees.amount", "autoWidth": true },
                { "data": "paid", "autoWidth": true },
                { "data": "balance", "autoWidth": true },
                {
                    "data": null, "render": function (data, type, full) {
                        if (data.status == "Completed") {
                            return `<span class="badge h4 bg-success" title="Fees completed">
                        <i class="bi bi-check2-circle"></i> Completed
                        </span>`;
                        } else {
                            return `<label title="Part Payment" class="badge h3 bg-warning"><i class="bi bi-star-half"></i> Part Payment</label>`;
                        }
                    }
                },
                {
                    "data": null, "render": function (data, type, full) {
                        return data.termregistrations.sessionYear.name;
                    }
                },
                {
                    "data": null, "render": function (data, type, full) {
                        return data.termregistrations.term;
                    }
                },
                {
                    "data": null, "render": function (data, type, full) {
                        return data.termregistrations.schoolclasses.name + " - " + data.termregistrations.subClasses.name;
                    }
                },
                {
                    "data": "createDate", "autoWidth": true,
                    render: function (data, type, row) {
                        var hours = new Date(data).getHours()
                        let ap = hours >= 12 ? 'pm' : 'am';
                        return data = data.toLocaleString('YYYY-MM-dd').slice(0, 19).replace('T', ' ') + ' ' + ap;
                    }
                }
            ]
        });
    }

    if ($("#other_payment").length) {
        $("#other_payment").DataTable({
            "processing": true,
            "serverSide": true,
            "filter": true,
            responsive: true,
            destroy: true,
            "ajax": {
                "url": "/api/otherPayemrnt",
                "type": "POST",
                "datatype": "json"
            },
            "columnDefs": [{ "visible": true, "searchable": true, }],
            "columns": [
                {
                    "data": "id", render: function (data, type, row, meta) { return meta.row + meta.settings._iDisplayStart + 1; }
                },
                {
                    "data": null, "render": function (data, type, full) {
                        var name = data.termregistration.studentsData.surName + ' ' + (data?.termregistration.studentsData.otherName || '') + ' ' + data.termregistration.studentsData.firstName;
                        return name;
                    }
                },
                { "data": "specialPay.amount", "autoWidth": true },
                { "data": "specialPay.otherPayTables.name", "autoWidth": true },
                { "data": "amount", "autoWidth": true },
                {
                    "data": null, "render": function (data, type, full) {
                        if (data.amount == data.specialPay.amount) {
                            return `<span class="badge h4 bg-success" title="Fees completed">
                        <i class="bi bi-check2-circle"></i> Completed
                        </span>`;
                        } else {
                            return `<label title="Part Payment" class="badge h3 bg-warning"><i class="bi bi-star-half"></i> Part Payment</label>`;
                        }
                    }
                },
                {
                    "data": null, "render": function (data, type, full) {
                        return data.termregistration.sessionYear.name;
                    }
                },
                {
                    "data": null, "render": function (data, type, full) {
                        return data.termregistration.term;
                    }
                },
                {
                    "data": null, "render": function (data, type, full) {
                        return data.termregistration.schoolclasses.name + " - " + data.termregistration.subClasses.name;
                    }
                },
                {
                    "data": "createdDate", "autoWidth": true,
                    render: function (data, type, row) {
                        var hours = new Date(data).getHours()
                        let ap = hours >= 12 ? 'pm' : 'am';
                        return data = data.toLocaleString('YYYY-MM-dd').slice(0, 19).replace('T', ' ') + ' ' + ap;
                    }
                }
            ]
        });

    }

    if ($("#pta-fees-table").length) {
        $("#pta-fees-table").DataTable({
            "processing": true,
            "serverSide": true,
            "filter": true,
            "paging": true,
            responsive: true,
            "ajax": {
                "url": "/api/PTAFeesPayemtData",
                "type": "POST",
                "datatype": "json"
            },
            "columnDefs": [{
                "targets": [0],
                "visible": true,
                "searchable": true,
            }],
            "columns": [
                {
                    "data": "id",
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                {
                    "data": null, "render": function (data, type, full) {
                        var name = data.termregistration.studentsData.surName + ' ' + (data?.termregistration.studentsData.otherName || '') + ' ' + data.termregistration.studentsData.firstName;
                        return name;
                    }
                },
                { "data": "ptaFee.fees", "autoWidth": true },
                { "data": "amount", "autoWidth": true },
                { "data": "balance", "autoWidth": true },
                { "data": "termregistration.term", "autoWidth": true },
                { "data": "termregistration.sessionYear.name", "autoWidth": true },
                {
                    "data": null, "render": function (data, type, full) {
                        var name = data.termregistration.schoolclasses.name + ' - ' + (data.termregistration.subClasses.name || '');
                        return name;
                    }
                },
                {
                    "data": "createdDate",
                    render: function (data, type, row) {
                        var hours = new Date(data).getHours()
                        let ap = hours >= 12 ? 'pm' : 'am';
                        return data = data.toLocaleString('YYYY-MM-dd').slice(0, 19).replace('T', ' ') + ' ' + ap;
                    }
                }
            ]
        });
    }
});
