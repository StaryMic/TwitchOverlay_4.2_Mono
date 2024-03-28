extends Node
@onready var db = SQLite.new()

func _ready():
	db.verbosity_level = db.VERBOSE
	db.path = "res://AvatarSettingsDB"
	db.open_db()

func DoesUserExistInDB(Username : String):
	var format = {"Username" : Username}
	db.query("SELECT * FROM AvatarSettings WHERE Username IS '{Username}'".format(format))
	if !db.query_result.is_empty():
		return true
	else:
		return false

func AddNewDataToDB(tablename : String, data_to_add : Dictionary):
	return db.insert_row(tablename,data_to_add)

func UpdateDataToDB(tablename : String, conditions : String, data_to_update : Dictionary):
	return db.update_rows(tablename,conditions,data_to_update)

func QueryDB(conditions : String):
	db.query(conditions)
	return db.query_result

func RemoveFromDB(tablename : String, conditions : String):
	return db.delete_rows(tablename,conditions)
