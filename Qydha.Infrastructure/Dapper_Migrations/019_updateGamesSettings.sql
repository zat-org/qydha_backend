ALTER TABLE user_baloot_settings
ADD is_sakkah_mashdodah_mode BOOLEAN DEFAULT FALSE  , 
ADD show_who_won_dialog_on_draw BOOLEAN DEFAULT FALSE ;

ALTER TABLE user_hand_settings
ADD win_using_zat BOOLEAN DEFAULT FALSE;