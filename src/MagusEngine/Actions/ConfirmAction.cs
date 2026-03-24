
                if (_targetCursor.AnyTargeted())
                {
                    bool sucess = false;
                    long timeTaken = 0;
                    if (_targetCursor.State == TargetState.LookMode)
                    {
                        _targetCursor.LookTarget();
                        return true;
                    }
                    else if (_targetCursor.State == TargetState.TargetingSpell)
                    {
                        (sucess, var spellCasted) = _targetCursor.EndSpellTargetting();
                        if (sucess)
                            timeTaken = TimeHelper.GetCastingTime(_getPlayer, spellCasted!);
                    }
                    else
                    {
                        (sucess, var item) = _targetCursor.EndItemTargetting();
                        if (sucess)
                            timeTaken = TimeHelper.GetShootingTime(_getPlayer, item!.Mass);
                    }
                    if (sucess)
                    {
                        _targetCursor = null;
                        Locator.GetService<MessageBusService>()?.SendMessage<ProcessTurnEvent>(new(timeTaken, sucess));
                    }

                    return sucess;
                }
                else
                {
                    Locator.GetService<MessageBusService>()?.SendMessage<AddMessageLog>(new("Invalid target!"));
                    return false;
                }

